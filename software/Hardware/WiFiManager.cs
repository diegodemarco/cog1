﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.Hardware
{
    public static class WiFiManager
    {

        public static WiFiReport GetWiFiStatus()
        {
            const string GENERAL_CONNECTION = "GENERAL.CONNECTION:";
            const string GENERAL_STATE = "GENERAL.STATE:";
            const string IP_V4 = "IP4.ADDRESS[1]:";

            /*
            GENERAL.DEVICE:                         wlan0
            GENERAL.TYPE:                           wifi
            GENERAL.HWADDR:                         10:34:37:AC:3B:2B
            GENERAL.MTU:                            1500
            GENERAL.STATE:                          50 (connecting (configuring))
            GENERAL.CONNECTION:                     Los De Marco
            GENERAL.CON-PATH:                       /org/freedesktop/NetworkManager/ActiveConnection/19
            IP4.GATEWAY:                            --
            IP6.GATEWAY:                            --
            */

            var result = new WiFiReport()
            {
                ssid = null,
                connectionState = 0,
                isConnected = false,
                savedConnections = GetConnections()
            };

            var output = OSUtils.RunWithOutput("nmcli", "device", "show", "wlan0");
            if (output == null)
                return result;

            var lines = output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // GENERAL_CONNECTION
            result.ssid = lines.FirstOrDefault(line => line.ToUpper().StartsWith(GENERAL_CONNECTION));
            if (!string.IsNullOrWhiteSpace(result.ssid))
            {
                result.ssid = result.ssid.Substring(GENERAL_CONNECTION.Length).Trim();
                if (string.Equals(result.ssid, "--", StringComparison.OrdinalIgnoreCase))
                    result.ssid = null;
            }

            // GENERAL_CONNECTION
            var generalState = lines.FirstOrDefault(line => line.ToUpper().StartsWith(GENERAL_STATE));
            if (!string.IsNullOrWhiteSpace(generalState))
            {
                generalState = generalState.Substring(GENERAL_STATE.Length).Trim();
                var parts = generalState.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && int.TryParse(parts[0], out var n))
                    result.connectionState = n;
            }
            //Console.WriteLine($"General.State={generalState}");
            //Console.WriteLine($"General.State.Int={result.connectionState}");
            result.isConnected = result.connectionState == 100;   // 100 means "connected"

            // If connected, add IP and signal information
            if (result.isConnected)
            {
                // IP4.ADDRESS[1]
                var ip4 = lines.FirstOrDefault(line => line.ToUpper().StartsWith(IP_V4));
                if (!string.IsNullOrWhiteSpace(ip4))
                {
                    ip4 = ip4.Substring(IP_V4.Length).Trim();
                    var parts = ip4.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out var mask))
                    {
                        result.ipv4 = parts[0];
                        result.maskBits = mask;
                    }
                    else if (parts.Length > 0)
                    {
                        result.ipv4 = parts[0];
                    }
                }

                // Signal
                GetSignal(out var rssi, out var noise, out var frequency);
                result.rssi = rssi;
                result.noise = noise;
                result.frequency = frequency;
            }

            return result;
        }

        #region Private

        private static bool initialized = false;

        private static List<string> GetConnections()
        {
            var data = OSUtils.ParseNMCliOutput(OSUtils.RunWithOutput("nmcli", "-t", "connection", "show"));
            return data.Where(line => line.Length >= 3 && line[2].Contains("802-11-wireless"))
                .Select(line => line[0])
                .ToList();
        }

        private static void GetSignal(out int rssi, out int noise, out int frequency)
        {
            /*
            RSSI=-73
            LINKSPEED=6553
            NOISE=9999
            FREQUENCY=5180
            */

            rssi = 0;
            noise = 0;
            frequency = 0;

            foreach (var line in OSUtils.GetLines(OSUtils.RunWithOutput("wpa_cli", "signal_poll")))
            {
                var parts = line.Split('=', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    if (string.Equals(parts[0], "RSSI", StringComparison.OrdinalIgnoreCase))
                    {
                        rssi = Convert.ToInt32(double.Parse(parts[1]));
                    }
                    else if (string.Equals(parts[0], "NOISE", StringComparison.OrdinalIgnoreCase))
                    {
                        noise = Convert.ToInt32(double.Parse(parts[1]));
                    }
                    else if (string.Equals(parts[0], "FREQUENCY", StringComparison.OrdinalIgnoreCase))
                    {
                        frequency = Convert.ToInt32(double.Parse(parts[1]));
                    }
                }
            }
        }

        private static void CheckWiFi()
        {
            if (!Directory.Exists("./wifi_log"))
                Directory.CreateDirectory("./wifi_log");
            for (; ; )
            {
                var wiFiStatus = WiFiManager.GetWiFiStatus();
                var wiFiText =
                    DateTime.UtcNow.ToString("s") + Environment.NewLine +
                    Environment.NewLine +
                    WiFiManager.GetWifiDetails() + Environment.NewLine +
                    Environment.NewLine +
                    JsonConvert.SerializeObject(wiFiStatus);

                //File.WriteAllText($"./wifi_log/{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.txt", wiFiText);

                if (!wiFiStatus.isConnected)
                {
                    ResetWiFi();
                    File.WriteAllText($"./wifi_log/{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.reset.txt", wiFiText);
                }

                Thread.Sleep(60000);
            }
        }

        private static bool ResetWiFi()
        {
            try
            {
                OSUtils.Run("nmcli", "radio", "wifi", "off");
                OSUtils.Run("systemctl", "stop", "NetworkManager");
                Thread.Sleep(5000);
                OSUtils.Run("systemctl", "start", "NetworkManager");
                OSUtils.Run("nmcli", "radio", "wifi", "on");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        public static void Init()
        {
            if (initialized) 
                return;
            Task.Run(() => CheckWiFi());
            initialized = true;
        }

        public static void Deinit()
        {
            initialized = false;
        }

        public static string GetWifiDetails()
        {
            // First from iwconfig
            string output = OSUtils.RunWithOutput("iwconfig", "wlan0");

            // Add nmcli information
            output += OSUtils.RunWithOutput("nmcli", "device", "show", "wlan0");

            // Add route information
            output += Environment.NewLine + OSUtils.RunWithOutput("route");

            // Add ip information
            output += Environment.NewLine + OSUtils.RunWithOutput("ip", "-4", "-o", "addr", "show", "wlan0");

            return output;
        }

    }
}