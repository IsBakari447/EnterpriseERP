using EnterpriseERP.ApiModels;
using EnterpriseERP.Data;
using EnterpriseERP.DTOs.Auth;
using EnterpriseERP.Helpers;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers.Api;

[Route("api/mobile/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordResetService _passwordResetService;

    public AuthController(ApplicationDbContext context, JwtService jwtService, PasswordResetService passwordResetService)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordResetService = passwordResetService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Email et mot de passe obligatoires.",
                Data = null,
                Errors = new List<string> { "Champs manquants." }
            });
        }

        var email = request.Email.Trim().ToUpperInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email);

        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Email ou mot de passe incorrect.",
                Data = null,
                Errors = new List<string> { "InvalidCredentials" }
            });
        }

        if (PasswordHelper.NeedsRehash(request.Password, user.PasswordHash))
            user.PasswordHash = PasswordHelper.HashPassword(request.Password);

        if (!user.IsActive)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Ce compte est desactive.",
                Data = null,
                Errors = new List<string> { "AccountDisabled" }
            });
        }

        if (!user.IsApproved)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Ce compte n'a pas encore ete approuve.",
                Data = null,
                Errors = new List<string> { "AccountNotApproved" }
            });
        }

        user.LastLogin = DateTime.UtcNow;
        user.LastConnection = DateTime.UtcNow;
        user.LoginCount += 1;
        user.LastIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Ok(new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Connexion API réussie.",
            Data = response,
            Errors = new List<string>()
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Nom, email et mot de passe obligatoires.",
                Data = null,
                Errors = new List<string> { "MissingFields" }
            });
        }

        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Les mots de passe ne correspondent pas.",
                Data = null,
                Errors = new List<string> { "PasswordMismatch" }
            });
        }

        var email = request.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();

        if (await _context.Users.AnyAsync(u => u.Email.ToUpper() == normalizedEmail))
        {
            return Conflict(new ApiResponse<object>
            {
                Success = false,
                Message = "Un compte existe déjà avec cet email.",
                Data = null,
                Errors = new List<string> { "EmailAlreadyExists" }
            });
        }

        var isFirstUser = !await _context.Users.AnyAsync();
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = PasswordHelper.HashPassword(request.Password),
            Role = isFirstUser ? "SuperAdmin" : "Employee",
            IsSuperAdmin = isFirstUser,
            IsActive = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PreferredLanguage = "fr",
            Theme = "Dark"
        };

        _context.Users.Add(user);

        if (!string.IsNullOrWhiteSpace(request.CompanyName) &&
            !await _context.CompanyProfiles.AnyAsync())
        {
            _context.CompanyProfiles.Add(new CompanyProfile
            {
                CompanyName = request.CompanyName.Trim(),
                Email = email
            });
        }

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);
        var response = new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Ok(new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Compte créé avec succès.",
            Data = response,
            Errors = new List<string>()
        });
    }

    [Authorize]
    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Token valide.",
            Data = new
            {
                User = User.Identity?.Name,
                ServerTime = DateTime.UtcNow
            },
            Errors = new List<string>()
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var result = await _passwordResetService.RequestResetAsync(request.Email, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new ApiResponse<object>
            {
                Success = false,
                Message = result.Message,
                Data = null,
                Errors = new List<string> { "PasswordResetRateLimited" }
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = result.Message,
            Data = null,
            Errors = new List<string>()
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var result = await _passwordResetService.ResetPasswordAsync(
            request.Email,
            request.Code,
            request.Password,
            request.ConfirmPassword,
            HttpContext.RequestAborted);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = result.Message,
                Data = null,
                Errors = new List<string> { "InvalidPasswordReset" }
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Mot de passe mis a jour avec succes.",
            Data = null,
            Errors = new List<string>()
        });
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API MOBILE OK");
    }
}
