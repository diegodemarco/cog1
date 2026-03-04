using cog1.BackgroundServices;
using cog1.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.System
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
        public class BackgroundTelemetryService(ILogger<BackgroundTelemetryService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "Background telemetry", LogCategory.System)
        {
            protected override async Task Run(CancellationToken stoppingToken)
            {
                var sw = Stopwatch.StartNew();
                var nextSec = 1000;

                await Task.Yield();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Global.IsDevelopment)
                        {
                            Utils.CancellableDelay(1000, stoppingToken);
                        }
                        else
                        {
                            Utils.CancellableDelay(800, stoppingToken);
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
                        Utils.CancellableDelay(1000, stoppingToken);
                    }
                }
            }
        }

    }

}
