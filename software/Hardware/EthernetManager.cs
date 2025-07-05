using cog1.DTO;
using cog1.Exceptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace cog1.Hardware
{
    public static class EthernetManager
    {

        #region Private

        private const string device_name = "end0";
        private const string connection_name = "ethernet";

        private static int GetLinkSpeed()
        {
            try
            {
                return int.Parse(File.ReadAllText($"/sys/class/net/{device_name}/speed"));
            }
            catch
            {
            }
            return 0;
        }

        private static bool GetLinkFullDuplex()
        {
            try
            {
                return File.ReadAllText($"/sys/class/net/{device_name}/duplex").Contains("full", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
            }
            return false;
        }

        private static bool GetLinkAutoNegotiate()
        {
            try
            {
                var line = OSUtils.GetLines(OSUtils.RunWithOutput("ethtool", device_name))
                    .FirstOrDefault(item => item.Trim().StartsWith("Auto-negotiation:", StringComparison.OrdinalIgnoreCase));
                if (line != null)
                {
                    var parts = line.Split(':', StringSplitOptions.TrimEntries);
                    if (parts.Length == 2)
                        return string.Equals(parts[1], "on", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
            }
            return false;
        }

        #endregion

        #region Ethernet status & information

        public static EthernetReport GetStatus()
        {
            /*
            This is an example of what the output of "nmcli device show end0" looks like:

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

            var result = new EthernetReport()
            {
                connectionState = 0,
                isConnected = false,
            };

            var dict = OSUtils.RunNmCli("GENERAL.DEVICE", "device", "show", device_name).FirstOrDefault();
            if (dict == null)
                return result;

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

            // Obtain link information from the kernel
            if (result.isConnected)
            {
                result.speed = GetLinkSpeed();
                result.fullDuplex = GetLinkFullDuplex();
                result.autoNegotiate = GetLinkAutoNegotiate();
            }

            return result;
        }

        #endregion

        #region Ethernet setup

        private static void RemoveEthernetConnections()
        {
            var dicts = OSUtils.RunNmCli("NAME", "conn", "show")
                .Where(item => item["DEVICE"].Equals(device_name, StringComparison.OrdinalIgnoreCase));
            foreach (var dict in dicts)
                OSUtils.Run("nmcli", "conn", "delete", dict["NAME"]);
        }

        public static bool Reconnect()
        {
            OSUtils.Run("nmcli", "conn", "down", "ethernet");
            return OSUtils.Run("nmcli", "conn", "up", "ethernet") == 0;
        }

        public static bool CheckEthernetConnection()
        {
            try
            {
                return OSUtils.RunWithOutput("nmcli", "conn", "show", connection_name).ToLower().Contains("connection.id:");
            }
            catch
            {
            }
            return false;
        }

        public static void EnsureEthernetConnection()
        {
            if (CheckEthernetConnection())
                return;
            Console.WriteLine("Setting up ethernet connection as 'ethernet'");
            RemoveEthernetConnections();
            OSUtils.Run("nmcli", "conn", "add", "connection.id", "ethernet", "ifname", "end0", "type", "ethernet",
                "802-3-ethernet.auto-negotiate", "yes", "ipv4.method", "auto");
            Reconnect();
        }

        #endregion

        #region Link configuration

        public static EthernetLinkConfigurationDTO GetLinkConfiguration()
        {
            var dict = OSUtils.RunNmCli("", "conn", "show", connection_name).FirstOrDefault();
            if (dict == null)
                return null;

            if (!dict.TryGetValue("802-3-ethernet.auto-negotiate", out var autoNegotiate))
                return null;

            if (string.Equals(autoNegotiate, "yes", StringComparison.OrdinalIgnoreCase))
                return new() { speed = 0 };

            if (dict.TryGetValue("802-3-ethernet.speed", out var speed) && int.TryParse(speed, out var intSpeed))
                return new() { speed = intSpeed };

            return new() { speed = 0 };
        }

        public static bool SetLinkConfiguration(EthernetLinkConfigurationDTO config, ErrorCodes ec)
        {
            if (config.speed != 0 && config.speed != 10 && config.speed != 100 && config.speed != 1000)
                throw new ControllerException(ec.General.INVALID_PARAMETER_VALUE("speed", config.speed.ToString()));

            var status = GetStatus();

            if (status.isConnected)
                OSUtils.Run("nmcli", "conn", "down", connection_name);

            if (config.speed == 0)
            {
                if (OSUtils.Run("nmcli", "conn", "modify", connection_name,
                    "802-3-ethernet.speed", string.Empty,
                    "802-3-ethernet.duplex", string.Empty,
                    "802-3-ethernet.auto-negotiate", "yes"
                    ) != 0)
                    return false;
            }
            else
            {
                if (OSUtils.Run("nmcli", "conn", "modify", connection_name,
                    "802-3-ethernet.speed", config.speed.ToString(),
                    "802-3-ethernet.duplex", "full",
                    "802-3-ethernet.auto-negotiate", "no"
                    ) != 0)
                    return false;
            }

            if (status.isConnected)
            {
                // Reconnect so that the changes take effect
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 15000)
                {
                    if (OSUtils.Run("nmcli", "conn", "up", connection_name) == 0)
                        return true;
                    Thread.Sleep(1000);
                }
                return false;
            }

            return true;
        }

        #endregion

        #region IP configuration

        public static IpConfigurationDTO GetIpConfiguration()
        {
            return OSUtils.GetIpConfiguration(connection_name);
        }

        public static bool SetIpConfiguration(IpConfigurationDTO config)
        {
            var status = GetStatus();

            if (!OSUtils.SetIpConfiguration(connection_name, config))
                return false;

            if (status.isConnected)
            {
                // Reconnect so that the changes take effect
                return Reconnect();
            }

            return true;
        }

        #endregion

    }
}
