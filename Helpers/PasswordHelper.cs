using System.Security.Cryptography;
using System.Text;

namespace EnterpriseERP.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword))
                return false;

            return storedPassword == HashPassword(password) || storedPassword == password;
        }

        public static bool NeedsRehash(string password, string storedPassword)
        {
            return !string.IsNullOrEmpty(storedPassword) && storedPassword == password;
        }
    }
}
