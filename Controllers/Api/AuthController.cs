using EnterpriseERP.ApiModels;
using EnterpriseERP.Data;
using EnterpriseERP.DTOs.Auth;
using EnterpriseERP.Helpers;
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

    public AuthController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
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

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API MOBILE OK");
    }
}
