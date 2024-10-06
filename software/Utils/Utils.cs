using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cog1
{
    public static class Utils
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 9 / 5 + 32;
        }

        public static double? CelsiusToFahrenheit(double? celsius)
        {
            if (celsius == null)
                return null;
            return celsius.Value * 9 / 5 + 32;
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

        public static async Task CancellableDelay(int milliseconds, CancellationToken ct)
        {
            await Task.Delay(milliseconds, ct).ContinueWith(task => { });
        }

        public static string DateTimeToSql(DateTime? dt)
        {
            if (dt == null)
                return null;
            return dt.Value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff", CultureInfo.InvariantCulture);
        }

        public static DateTime? SqlToDateTime(string dt)
        {
            if (dt == null)
                return null;
            return DateTime.Parse(dt, CultureInfo.InvariantCulture);
        }

    }
}
