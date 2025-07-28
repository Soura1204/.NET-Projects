using BCrypt.Net;

namespace RegistrationApp.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // ✅ Same as in API
        }
    }
}
