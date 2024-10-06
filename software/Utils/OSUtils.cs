using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cog1
{
    public static class OSUtils
    {
        static bool rebooting = false;

        public static bool Rebooting => rebooting;

        public static string RunWithOutput(string fileName, params string[] parameters)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                ArgumentList = { },
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            if (parameters != null)
                foreach (var item in parameters)
                    psi.ArgumentList.Add(item);

            string output;
            using (var process = Process.Start(psi))
            {
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            return output;
        }

        public static void Run(string fileName, params string[] parameters)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                ArgumentList = { },
                UseShellExecute = false,
                RedirectStandardOutput = false,
            };
            if (parameters != null)
                foreach (var item in parameters)
                    psi.ArgumentList.Add(item);

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }

        public static List<string[]> ParseNMCliOutput(string lines)
        {
            if (string.IsNullOrWhiteSpace(lines))
                return new List<string[]>();

            var result = new List<string[]>();
            var pLines = GetLines(lines
                .Replace("\\\\", "\\")
                .Replace("\\:", ":"));
            foreach (var line in pLines)
                result.Add(line.Split(':'));

            return result;
        }

        public static Dictionary<string, string> ParseValuePairs(string lines)
        {
            if (string.IsNullOrWhiteSpace(lines))
                return new();

            var result = new Dictionary<string, string>();
            foreach (var line in GetLines(lines))
            {
                var parts = line.Split('=', 1, StringSplitOptions.TrimEntries);
                if (parts.Length > 0)
                {
                    if (parts.Length == 2)
                    {
                        result.Add(parts[0], parts[1]);
                    }
                    else
                    {
                        result.Add(parts[0], "");
                    }
                }
            }
            return result;
        }

        public static string GetValuePair(string lines, string key)
        {
            var dict = ParseValuePairs(lines);
            foreach (var item in dict)
            {
                if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return item.Value;
            }
            return string.Empty;
        }

        public static List<string> GetLines(string s, bool removeEmpty = false, bool trim = false)
        {
            if (string.IsNullOrWhiteSpace(s))
                return new List<string>();

            StringSplitOptions options = StringSplitOptions.None;
            if (removeEmpty)
                options |= StringSplitOptions.RemoveEmptyEntries;
            if (trim)
                options |= StringSplitOptions.TrimEntries;

            return s
                .Replace("\r", "")
                .Split('\n', options)
                .ToList();
        }

        //public static bool GetRebooting()
        //{
        //    const string shutdown_file = "/run/systemd/shutdown/scheduled";
        //    if (File.Exists(shutdown_file))
        //    {
        //        try
        //        {
        //            var content = File.ReadAllText(shutdown_file);
        //            Console.WriteLine(content);
        //            File.WriteAllText($"./down.{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.txt", content);
        //            return string.Equals(GetValuePair(content, "MODE"), "reboot", StringComparison.OrdinalIgnoreCase);
        //        }
        //        catch (Exception e)
        //        {
        //            File.WriteAllText($"./err.{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.txt", e.ToString());
        //        }
        //    }
        //    else
        //    {
        //        File.WriteAllText($"./down.{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.txt", $"{shutdown_file} does not exist");
        //    }
        //    return false;
        //}

        public static void Reboot()
        {
            rebooting = true;
            Run("reboot", "now");
        }

        public static void Shutdown()
        {
            Run("shutdown", "now");
        }

    }
}
