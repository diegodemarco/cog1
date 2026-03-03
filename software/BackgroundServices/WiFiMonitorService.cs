using cog1.BackgroundServices;
using cog1.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.System
{
    /// <summary>
    /// The WiFi monitor service lives inside the WiFiManager singleton, to 
    /// verify the status of the WiFi network and try to reconnect when
    /// connection is unexpectedly lost.
    /// This class is nested inside the WiFiManager to have access to private 
    /// WiFiManager fields and methods.
    /// </summary>
    /// <param name="logger">logger used by the background service</param>
    public class WiFiMonitorService(ILogger<WiFiMonitorService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "WiFi monitor", LogCategory.System)
    {
        protected override async Task Run(CancellationToken stoppingToken)
        {
            if (!Directory.Exists("./wifi_log"))
                Directory.CreateDirectory("./wifi_log");

            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (WiFiManager.NeedsReset())
                    {
                        var status = WiFiManager.GetStatus();
                        var wiFiText =
                            DateTime.UtcNow.ToString("s") + Environment.NewLine +
                            Environment.NewLine +
                            WiFiManager.GetWifiDetails() + Environment.NewLine +
                            Environment.NewLine +
                            JsonConvert.SerializeObject(status);

                        //File.WriteAllText($"./wifi_log/{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.txt", wiFiText);

                        ResetWiFi();
                        File.WriteAllText($"./wifi_log/{DateTime.UtcNow.ToString("yyyyMMdd.HHmmss")}.reset.txt", wiFiText);
                    }

                    // Wait for 60 seconds
                    Utils.CancellableDelay(60000, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error in WiFi monitor service: {ex}");
                    Utils.CancellableDelay(1000, stoppingToken);
                }
            }
        }

        private static bool ResetWiFi()
        {
            try
            {
                OSUtils.Run("rmmod", "sprdwl_ng");
                OSUtils.Run("modprobe", "sprdwl_ng");
                /*
                OSUtils.Run("nmcli", "radio", "wifi", "off");
                OSUtils.Run("systemctl", "stop", "NetworkManager");
                Thread.Sleep(5000);
                OSUtils.Run("systemctl", "start", "NetworkManager");
                OSUtils.Run("nmcli", "radio", "wifi", "on");
                OSUtils.Run("systemctl", "restart", "wpa_supplicant");
                Thread.Sleep(5000);
                OSUtils.Run("systemctl", "start", "wpa_supplicant");
                */
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}