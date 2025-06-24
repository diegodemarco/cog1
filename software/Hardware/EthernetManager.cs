using cog1.Business;
using cog1.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using static cog1.Literals.NetworkLiterals;

namespace cog1.Hardware
{
    public static partial class EthernetManager
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
            OSUtils.ParseNMCliDeviceShow(dict, out var connState, out var isConnected, out var ipv4, out var maskBits, out var gateway, out var dns, out var macAddress);
            result.macAddress = macAddress;
            result.connectionState = connState;
            result.isConnected = isConnected;
            result.ipv4 = ipv4;
            result.maskBits = maskBits;
            result.gateway = gateway;
            result.dns = dns;

            // IP configuration obtained via ip
            OSUtils.GetIpData(device_name, out var isDynamic);
            result.dhcp = isDynamic;

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

        public static void Reconnect()
        {
            OSUtils.Run("nmcli", "conn", "down", "ethernet");
            OSUtils.Run("nmcli", "conn", "up", "ethernet");
        }

        public static void SetFixedIP(string ipv4, int netMask, string gateway, string dns)
        {
            RemoveEthernetConnections();
            OSUtils.Run("nmcli", "conn", "modify", connection_name, 
                "ipv4.method", "manual", "ipv4.addresses", $"{ipv4}/{netMask}", "ipv4.gateway", gateway, "ipv4.dns", dns);
            Reconnect();
        }

        public static void SetDHCP()
        {
            RemoveEthernetConnections();
            OSUtils.Run("nmcli", "conn", "modify", connection_name, 
                "ipv4.method", "auto", "ipv4.addresses", string.Empty, "ipv4.gateway", string.Empty, "ipv4.dns", string.Empty);
            Reconnect();
        }

        public static void SetSpeed(int speed, bool fullDuplex, ErrorCodes ec)
        {
            if (speed != 10 && speed != 100 && speed != 1000)
                throw new ControllerException(ec.General.INVALID_PARAMETER_VALUE("speed", speed.ToString()));
            OSUtils.Run("nmcli", "conn", "modify", connection_name,
                "802-3-ethernet.speed", speed.ToString(),
                "802-3-ethernet.duplex", fullDuplex ? "full" : "half",
                "802-3-ethernet.auto-negotiate", "no"
                );
            Reconnect();
        }

        public static void SetSpeedAuto()
        {
            OSUtils.Run("nmcli", "conn", "modify", connection_name,
                "802-3-ethernet.speed", string.Empty,
                "802-3-ethernet.duplex", string.Empty,
                "802-3-ethernet.auto-negotiate", "yes"
                );
            Reconnect();
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

    }
}
