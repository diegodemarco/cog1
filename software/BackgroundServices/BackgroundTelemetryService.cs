using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;

namespace cog1.Telemetry
{
    public static partial class SystemStats
    {

        /// <summary>
        /// The telemetry background service lives inside the SystemStats singleton, 
        /// to keep telemetry data updated every second.
        /// This class is nested inside the SystemStats class to have access to private 
        /// SystemStats fields and methods.
        /// </summary>
        /// <param name="logger">logger used by the background service</param>
        public class BackgroundTelemetryService(ILogger<BackgroundTelemetryService> logger) : BackgroundService
        {
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                logger.LogInformation("Background telemetry service started");

                var sw = Stopwatch.StartNew();
                var nextSec = 1000;

                // Signal that the background task has started
                await Utils.CancellableDelay(1000, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Global.IsDevelopment)
                        {
                            await Utils.CancellableDelay(1000, stoppingToken);
                        }
                        else
                        {
                            await Utils.CancellableDelay(800, stoppingToken);
                            if (!stoppingToken.IsCancellationRequested)
                            {
                                while (sw.ElapsedMilliseconds < nextSec)
                                    Thread.Sleep(10);
                                nextSec += 1000;
                                SystemStats.Collect1SecondData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Background telemetry error: {ex}");
                        await Utils.CancellableDelay(1000, stoppingToken);
                    }
                }

                logger.LogInformation("Background telemetry service stopped");
            }
        }

    }

}
