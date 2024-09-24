using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace cog1app
{
    public static class BackgroundChecker
    {

        public static void Init()
        {
            Task.Run(() => CheckWiFi());
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

        public static void Deinit()
        {

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

    }
}
