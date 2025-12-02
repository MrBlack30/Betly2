using System.Security.Cryptography;
using System.Text;

namespace Betly.Api.Utilities
{
    public static class PasswordHasher
    {
        // For demonstration purposes ONLY. Use a real hashing library in production.
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}