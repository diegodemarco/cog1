using cog1.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Hardware
{
    public static class WiFiManager
    {

        #region Private

        private const string device_name = "wlan0";

        private class ScanItem
        {
            public string ssid { get; set; }
            public int quality { get; set; }
            public int channel { get; set; }
            public string security { get; set; }
        }

        private static HashSet<int> channels_24 = new() 
        { 
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 
        };
        private static HashSet<int> channels_5 = new()
        {
            32, 36, 40, 44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 84, 88, 92, 96, 100, 104, 108,
            112, 116, 120, 124, 128, 132, 136, 140, 144, 149, 153, 157, 161, 165, 169, 173, 177
        };

        public static List<string> GetConnections()
        {
            return OSUtils.RunNmCli("NAME", "connection", "show")
                .Where(item => item["TYPE"].Equals("802-11-wireless", StringComparison.OrdinalIgnoreCase))
                .Select(item => item["NAME"])
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

        #endregion

        #region WiFi status & information

        public static WiFiReport GetStatus()
        {
            /*
            This is an example of what the output of "nmcli device show wlan0" looks like:

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

            /*
            const string GENERAL_CONNECTION = "GENERAL.CONNECTION:";
            const string GENERAL_STATE = "GENERAL.STATE:";
            const string IP_V4 = "IP4.ADDRESS[1]:";
            */

            var result = new WiFiReport()
            {
                ssid = null,
                connectionState = 0,
                isConnected = false,
                savedConnections = GetConnections()
            };

            var dict = OSUtils.RunNmCli("GENERAL.DEVICE", "device", "show", device_name).FirstOrDefault();
            if (dict == null)
                return result;

            // GENERAL.CONNECTION
            if (dict.TryGetValue("GENERAL.CONNECTION", out var ssid))
                result.ssid = ssid;

            // IP configuration obtained via nmcli
            OSUtils.ParseNMCliDeviceShow(dict, out var connState, out var isConnected, out var ipv4, out var netMask, out var gateway, out var dns, out var macAddress);
            result.macAddress = macAddress;
            result.connectionState = connState;
            result.isConnected = isConnected;

            // IP configuration obtained via ip
            result.ipConfiguration = new()
            {
                dhcp = OSUtils.IsDynamicIp(device_name),
                ipv4 = ipv4,
                netMask = netMask,
                gateway = gateway,
                dns = dns
            };

            if (result.isConnected)
            {
                // Signal
                GetSignal(out var rssi, out var noise, out var frequency);
                result.rssi = rssi;
                result.noise = noise;
                result.frequency = frequency;
            }

            return result;
        }

        public static string GetWifiDetails()
        {
            // First from iw
            string output = OSUtils.RunWithOutput("iw", "dev", device_name, "link");

            // Add nmcli information
            output += OSUtils.RunWithOutput("nmcli", "device", "show", device_name);

            // Add route information
            output += Environment.NewLine + OSUtils.RunWithOutput("route");

            // Add ip information
            output += Environment.NewLine + OSUtils.RunWithOutput("ip", "-4", "-o", "addr", "show", device_name);

            return output;
        }

        public static List<WiFiSsidDTO> Scan()
        {
            var scanList = new List<ScanItem>();
            var nets = OSUtils.RunNmCli("IN-USE", "dev", "wifi", "list", "--rescan", "yes");
            foreach (var net in nets)
            {
                if (net.TryGetValue("SSID", out var ssid) && !string.IsNullOrWhiteSpace(ssid)
                    && net.TryGetValue("MODE", out var mode) && string.Equals(mode, "Infra", StringComparison.OrdinalIgnoreCase)
                    && net.TryGetValue("SIGNAL", out var signal) && int.TryParse(signal, out var signalInt)
                    && net.TryGetValue("CHAN", out var channel) && int.TryParse(channel, out var channelInt)
                    && net.TryGetValue("SECURITY", out var security))
                {
                    scanList.Add(new()
                    {
                        ssid = ssid,
                        channel = channelInt,
                        quality = signalInt,
                        security = security
                    });
                }
            }

            var status = GetStatus();
            var ssids = status.savedConnections.Concat(scanList.Select(item => item.ssid)).ToHashSet();     // Unique SSIDs
            var result = new List<WiFiSsidDTO>();
            foreach (var ssid in ssids)
            {
                var scans = scanList.Where(item => string.Equals(item.ssid, ssid)).ToList();
                result.Add(new()
                {
                    ssid = ssid,
                    isConnected = status.isConnected && string.Equals(ssid, status.ssid),
                    isSaved = status.savedConnections.Contains(ssid),
                    frequencies = GetFrequencies(scans),
                    quality = GetQuality(scans),
                    isOpen = !scans.Any(item => item.ssid.Equals(ssid) && !string.IsNullOrWhiteSpace(item.security))
                });
            }

            return result
                .OrderBy(item => item.isConnected ? 0 : 1)
                .ThenBy(item => item.isSaved ? 0 : 1)
                .ThenByDescending(item => item.quality)
                .ToList();
        }

        private static string GetFrequencies(List<ScanItem> scans)
        {
            if (scans.Count == 0)
                return string.Empty;
            var is24 = false;
            var is5 = false;
            foreach (var ch in scans.Select(item => item.channel))
            {
                if (channels_24.Contains(ch))
                    is24 = true;
                if (channels_5.Contains(ch))
                    is5 = true;
            }
            if (is24 && is5)
                return "5 GHz / 2.4 GHz";
            if (is5)
                return "5 GHz";
            if (is24)
                return "2.4 GHz";
            return string.Empty;
        }

        private static int GetQuality(List<ScanItem> scans)
        {
            if (scans.Count == 0)
                return 0;
            return scans.Select(item => item.quality).Max();
        }

        #endregion

        #region WiFi ssid connect/disconnect/reconnect/forget

        public static bool Reconnect(string ssid)
        {
            OSUtils.Run("nmcli", "conn", "modify", ssid, "connection.autoconnect", "yes");
            return OSUtils.Run("nmcli", "conn", "up", ssid) == 0;
        }

        public static bool Connect(string ssid, string password)
        {
            Forget(ssid);
            if (OSUtils.Run("nmcli", "device", "wifi", "connect", ssid, "password", password) == 0)
                return true;
            // Failure
            Forget(ssid);
            return false;
        }

        public static bool Disconnect(string ssid)
        {
            return OSUtils.Run("nmcli", "conn", "down", ssid) == 0;
        }

        public static bool Forget(string ssid)
        {

            return OSUtils.Run("nmcli", "conn", "delete", ssid) == 0;
        }

        #endregion

        #region IP configuration

        public static IpConfigurationDTO GetIpConfiguration(string ssid)
        {
            return OSUtils.GetIpConfiguration(ssid);
        }

        public static bool SetIpConfiguration(string ssid, IpConfigurationDTO config)
        {
            // Get current status
            var status = GetStatus();

            if (!OSUtils.SetIpConfiguration(ssid, config))
                return false;

            // If this was connected, reconnect it so it uses the new parameters
            if (status.ssid == ssid)
                return Reconnect(ssid);
            return true;
        }

        #endregion

    }
}
