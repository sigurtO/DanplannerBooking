using System;
using System.Security.Cryptography;

namespace DanplannerBooking.Domain.Security
{
    /// <summary>
    /// Password hashing helper baseret på PBKDF2 (Rfc2898DeriveBytes).
    /// Format i databasen: {iterations}.{base64-salt}.{base64-hash}
    /// </summary>
    public static class PasswordHasher
    {
        // Kan justeres hvis det ønskes
        private const int SaltSize = 16;    // 128 bit
        private const int KeySize = 32;    // 256 bit
        private const int Iterations = 100_000;

        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            // Generer tilfældig salt
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Deriver nøgle ud fra password + salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            // Gem som "iterations.salt.hash" (salt og hash i Base64)
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword)) return false;

            // Bagud-kompatibilitet:
            // Hvis den ikke ligner vores hash-format, antager vi at det er klartekst.
            if (!IsHashed(storedPassword))
            {
                return storedPassword == password;
            }

            var parts = storedPassword.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations))
                return false;

            var salt = Convert.FromBase64String(parts[1]);
            var storedKey = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password!, salt, iterations, HashAlgorithmName.SHA256);
            var keyToCheck = pbkdf2.GetBytes(storedKey.Length);

            // Constant-time sammenligning
            return CryptographicOperations.FixedTimeEquals(storedKey, keyToCheck);
        }

        public static bool IsHashed(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var parts = password.Split('.');
            if (parts.Length != 3) return false;

            return int.TryParse(parts[0], out _);
        }
    }
}
