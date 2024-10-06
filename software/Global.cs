using System;
using System.IO;
using System.Threading.Tasks;

namespace cog1
{
    public static class Global
    {
        private static readonly string exeDir = null;
        private static readonly string dataDir = null;

        public const string Version = "1.0.0.0";

        private static bool? isDevelopment = null;

        static Global()
        {
            // Get the directory of the main executable
            using (var p = System.Diagnostics.Process.GetCurrentProcess())
                exeDir = Path.GetDirectoryName(p.MainModule.FileName);

            // Calculate all other relative paths
            dataDir = Path.Combine(exeDir, "data");

            // Create any missing directories
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);
        }

        public static bool IsDevelopment => GetIsDevelopment();

        public static string ExecutableDirectory => exeDir;
        public static string DataDirectory => dataDir;

        private static bool GetIsDevelopment()
        {
            if (isDevelopment == null)
                isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
            return isDevelopment.Value;
        }

    }
}
