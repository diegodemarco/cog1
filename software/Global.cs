using System;

namespace cog1
{
    public static class Global
    {
        public const string Version = "1.0.0.0";

        private static bool? isDevelopment = null;

        public static bool IsDevelopment => GetIsDevelopment();

        private static bool GetIsDevelopment()
        {
            if (isDevelopment == null)
                isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
            return isDevelopment.Value;
        }

    }
}
