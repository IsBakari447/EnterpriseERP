using System.Security.Cryptography;
using System.Text;
namespace EnterpriseERP.Services;

public sealed class PasswordResetTokenService
{
    public string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    public string HashCode(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
        return Convert.ToBase64String(bytes);
    }

    public bool Matches(string code, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var tokenHash = HashCode(code.Trim());
        var tokenBytes = Encoding.UTF8.GetBytes(tokenHash);
        var storedBytes = Encoding.UTF8.GetBytes(storedHash);
        return CryptographicOperations.FixedTimeEquals(tokenBytes, storedBytes);
    }
}
