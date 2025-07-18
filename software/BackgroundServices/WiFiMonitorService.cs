﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using Newtonsoft.Json;
using System.IO;

namespace cog1.Hardware
{
    /// <summary>
    /// The WiFi monitor service lives inside the WiFiManager singleton, to 
    /// verify the status of the WiFi network and try to reconnect when
    /// connection is unexpectedly lost.
    /// This class is nested inside the WiFiManager to have access to private 
    /// WiFiManager fields and methods.
    /// </summary>
    /// <param name="logger">logger used by the background service</param>
    public class WiFiMonitorService(ILogger<WiFiMonitorService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("WiFi monitor service started");

            if (!Directory.Exists("./wifi_log"))
                Directory.CreateDirectory("./wifi_log");

            // Signal that the background task has started
            await Utils.CancellableDelay(1000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!Global.IsDevelopment && WiFiManager.GetConnections().Count > 0)
                    {
                        var wiFiStatus = WiFiManager.GetStatus();
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
                    }

                    // Wait for 60 seconds
                    await Utils.CancellableDelay(60000, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error in WiFi monitor service: {ex}");
                    await Utils.CancellableDelay(1000, stoppingToken);
                }
            }

            logger.LogInformation("WiFi monitor service stopped");
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