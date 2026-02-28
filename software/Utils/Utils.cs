using System;
using System.Diagnostics.Contracts;
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

        public static void CancellableDelay(int milliseconds, CancellationToken ct)
        {
            ct.WaitHandle.WaitOne(milliseconds);
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

        public static string BytesToHex(byte[] bytes, string separator = "")
        {
            if (bytes == null || bytes.Length < 1)
                return "";
            return BytesToHex(bytes, bytes.Length, separator);
        }

        public static string BytesToHex(byte[] bytes, int len, string separator = "")
        {
            if (bytes == null || bytes.Length < 1)
                return "";
            return BitConverter.ToString(bytes, 0, len).Replace("-", separator);
        }

        public static bool ValidateHttpUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;
            return (uri.Scheme == "http" || uri.Scheme == "https");
        }

        public static bool ValidateMqttHost(string host)
        {
            return SplitMqttHost(host, out _, out _);
        }

        public static bool SplitMqttHost(string host, out string hostName, out int port)
        {
            hostName = string.Empty;
            port = 0;
            if (string.IsNullOrWhiteSpace(host))
                return false;
            if (host.Contains("/") || host.Contains("?"))
                return false;
            if (host.Contains("/"))
                return false;
            if (!Uri.TryCreate("http://" + host, UriKind.Absolute, out _))
                return false;
            var parts = host.Split(':');
            if (parts.Length == 1)
            {
                hostName = host;
                port = 1883;
                return true;
            }
            else if (parts.Length == 2)
            {
                hostName = parts[0];
                return int.TryParse(parts[1], out port);
            }
            else
            {
                return false;
            }
        }

    }
}
