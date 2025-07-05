using cog1.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace cog1
{
    public enum RebootStatus
    {
        Shutdown = 0,
        Reinit = 1,
        Reboot = 2,
    }

    public static class OSUtils
    {
        static RebootStatus rebootStatus = RebootStatus.Shutdown;

        public static RebootStatus RebootStatus => rebootStatus;

        public static string RunWithOutput(string fileName, params string[] parameters)
        {
            return RunWithOutput(out _, fileName, parameters);
        }

        public static string RunWithOutput(out int exitCode, string fileName, params string[] parameters)
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
                exitCode = process.ExitCode;
            }

            return output;
        }

        public static int Run(string fileName, params string[] parameters)
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
                return process.ExitCode;
            }
        }

        public static List<Dictionary<string, string>> RunNmCli(string firstItem, params string[] p)
        {
            var rawData = OSUtils.RunWithOutput("nmcli", new string[3] { "-t", "--mode", "multiline" }.Concat(p).ToArray());
            //Console.WriteLine(rawData);
            return OSUtils.ParseNMCliMultiLine(rawData, firstItem);
        }

        private static List<Dictionary<string, string>> ParseNMCliMultiLine(string lines, string firstKey)
        {
            if (string.IsNullOrWhiteSpace(lines))
                return new();

            var result = new List<Dictionary<string, string>>();
            var pLines = GetLines(lines);
            var dict = new Dictionary<string, string>();
            foreach (var line in pLines)
            {
                var sepIndex = line.IndexOf(':');
                if (sepIndex > 0)
                {
                    var key = line.Substring(0, sepIndex);
                    var value = (line.Length > sepIndex + 1) ? line.Substring(sepIndex + 1) : string.Empty;
                    if (dict.Count > 0 && key.Equals(firstKey, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(dict);
                        dict = new Dictionary<string, string>();
                    }
                    dict.Add(key, value);
                }
            }
            // Add last dict
            if (dict.Count > 0)
                result.Add(dict);
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
            rebootStatus = RebootStatus.Reboot;
            Run("reboot", "now");
        }

        public static void Reinit()
        {
            rebootStatus = RebootStatus.Reinit;
            Run("systemctl", "restart", "cog1");
        }

        public static void Shutdown()
        {
            rebootStatus = RebootStatus.Shutdown;
            Run("shutdown", "now");
        }

        public static void ParseNMCliDeviceShow(Dictionary<string, string> dict, out int connState, out bool isConnected, 
            out string ipv4, out int maskBits, out string gateway, out string dns, out string macAddress)
        {
            connState = 0;
            ipv4 = string.Empty;
            maskBits = 0;
            gateway = string.Empty;
            dns = string.Empty;

            // GENERAL.STATE
            if (dict.TryGetValue("GENERAL.STATE", out var generalState))
            {
                var parts = generalState.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && int.TryParse(parts[0], out var n))
                    connState = n;
            }
            //Console.WriteLine($"General.State={generalState}");
            //Console.WriteLine($"General.State.Int={result.connectionState}");
            isConnected = connState == 100;   // 100 means "connected"

            // MAC address
            if (!dict.TryGetValue("GENERAL.HWADDR", out macAddress))
                macAddress = string.Empty;

            // If connected, add IP and signal information
            if (isConnected)
            {
                // IP4.ADDRESS[1]
                if (dict.TryGetValue("IP4.ADDRESS[1]", out var ip4) && !string.IsNullOrWhiteSpace(ip4))
                {
                    var parts = ip4.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out var mask))
                    {
                        ipv4 = parts[0];
                        maskBits = mask;
                    }
                    else if (parts.Length > 0)
                    {
                        ipv4 = parts[0];
                    }
                }

                // IP4.GATEWAY
                if (dict.TryGetValue("IP4.GATEWAY", out var gw4) && !string.IsNullOrWhiteSpace(gw4))
                    gateway = gw4;

                // IP4.DNS[1]
                if (dict.TryGetValue("IP4.DNS[1]", out var dns4) && !string.IsNullOrWhiteSpace(dns4))
                    dns = dns4;
            }
        }

        public static IpConfigurationDTO GetIpConfiguration(string connName)
        {
            var dict = OSUtils.RunNmCli("connection.id", "conn", "show", connName).FirstOrDefault();
            if (dict == null)
                return null;
            //Console.WriteLine(JsonConvert.SerializeObject(dict));

            var result = new IpConfigurationDTO() { dhcp = true, ipv4 = string.Empty, netMask = 0, gateway = string.Empty, dns = string.Empty };

            // ipv4.method
            result.dhcp = dict.TryGetValue("ipv4.method", out var method) && string.Equals(method, "auto", StringComparison.OrdinalIgnoreCase);
            if (result.dhcp)
                return result;

            // ipv4.addresses
            if (dict.TryGetValue("ipv4.addresses", out var ip4) && !string.IsNullOrWhiteSpace(ip4) && ip4 != "--")
            {
                var parts = ip4.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && int.TryParse(parts[1], out var mask))
                {
                    result.ipv4 = parts[0];
                    result.netMask = mask;
                }
                else if (parts.Length > 0)
                {
                    result.ipv4 = parts[0];
                }
            }

            // ipv4.gateway
            if (dict.TryGetValue("ipv4.gateway", out var gw4) && !string.IsNullOrWhiteSpace(gw4) && gw4 != "--")
                result.gateway = gw4;

            // ipv4.dns
            if (dict.TryGetValue("ipv4.dns", out var dns4) && !string.IsNullOrWhiteSpace(dns4) && dns4 != "--")
                result.dns = dns4;

            // Done
            return result;
        }

        public static bool IsDynamicIp(string deviceName)
        {
            var data = GetLines(RunWithOutput("ip", "-4", "addr", "show", deviceName))
                .FirstOrDefault(item => item.Trim().StartsWith("inet "));
            return (data != null) && (data.Contains(" dynamic ") || data.EndsWith(" dynamic"));
        }

        public static string QuoteString(string s)
        {
            return $"\"{s.Replace("\"", "\\\"")}\"";
        }

        public static bool SetIpConfiguration(string connName, IpConfigurationDTO config)
        {
            // Configure
            if (config.dhcp)
            {
                return (OSUtils.Run("nmcli", "conn", "modify", connName,
                    "ipv4.method", "auto",
                    "ipv4.addresses", string.Empty,
                    "ipv4.gateway", string.Empty,
                    "ipv4.dns", string.Empty) == 0);
            }
            else
            {
                return (OSUtils.Run("nmcli", "conn", "modify", connName,
                    "ipv4.addresses", $"{config.ipv4}/{config.netMask}",
                    "ipv4.gateway", config.gateway,
                    "ipv4.dns", config.dns,
                    "ipv4.method", "manual") == 0);
            }
        }

    }

}
