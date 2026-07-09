using EnterpriseERP.ApiModels;
using EnterpriseERP.DTOs.Auth;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers.Api;

[Route("api/mobile/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
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

        var userId = 1;
        var fullName = "Issa Bakari";
        var role = "Admin";

        var token = _jwtService.GenerateToken(userId, fullName, request.Email, role);

        var response = new LoginResponseDto
        {
            UserId = userId,
            FullName = fullName,
            Email = request.Email,
            Role = role,
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
