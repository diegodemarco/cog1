using System;
using System.Security.Cryptography;
using System.Text;

namespace cog1
{
    public static class Utils
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return (celsius * 9 / 5) + 32;
        }

        public static double? CelsiusToFahrenheit(double? celsius)
        {
            if (celsius == null)
                return null;
            return (celsius.Value * 9 / 5) + 32;
        }

        public static string HashPassword(int user_id, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user_id.ToString() + password));
                return Convert.ToBase64String(bytes);
            }
        }

    }
}
