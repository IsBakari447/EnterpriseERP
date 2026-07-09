using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseERP.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(int userId, string fullName, string email, string role)
    {
        var jwt = _config.GetSection("Jwt");
        var jwtSettings = _config.GetSection("JwtSettings");

        var secretKey = jwt["Key"] ?? jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = jwt["Issuer"] ?? jwtSettings["Issuer"] ?? "EnterpriseERP";
        var audience = jwt["Audience"] ?? jwtSettings["Audience"] ?? "EnterpriseERP.Mobile";
        var expirationText = jwt["ExpireMinutes"] ?? jwtSettings["ExpirationInMinutes"];
        var expirationMinutes = int.TryParse(expirationText, out var minutes) ? minutes : 1440;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
