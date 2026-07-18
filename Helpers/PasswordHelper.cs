using System.Security.Cryptography;
using System.Text;

namespace EnterpriseERP.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private const string Pbkdf2Prefix = "PBKDF2";

        public static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return string.Join(
                ".",
                Pbkdf2Prefix,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        public static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedPassword))
                return false;

            if (storedPassword.StartsWith($"{Pbkdf2Prefix}.", StringComparison.Ordinal))
                return VerifyPbkdf2(password, storedPassword);

            return FixedTimeEquals(storedPassword, HashLegacySha256(password));
        }

        public static bool NeedsRehash(string password, string storedPassword)
        {
            return !string.IsNullOrWhiteSpace(storedPassword)
                && !storedPassword.StartsWith($"{Pbkdf2Prefix}.", StringComparison.Ordinal);
        }

        private static bool VerifyPbkdf2(string password, string storedPassword)
        {
            var parts = storedPassword.Split('.');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations))
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expectedKey = Convert.FromBase64String(parts[3]);
                var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedKey.Length);

                return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static string HashLegacySha256(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);
            return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }
    }
}
