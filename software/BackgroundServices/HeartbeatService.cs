using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using cog1.Telemetry;

namespace cog1.Hardware
{
    public static partial class IOManager
    {

        /// <summary>
        /// Heartbeat baground task lives inside the IOManager singleton, to 
        /// update the power LED simulating a heartbeat, with its speed 
        /// reflecting current CPU load.
        /// This class is nested inside the IOManager to have access to private 
        /// IOManager fields and methods.
        /// </summary>
        /// <param name="logger">logger used by the background service</param>
        public class HeartbeatService(ILogger<HeartbeatService> logger) : BackgroundService
        {
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                const int short_sleep_min = 40;
                const int short_sleep_max = 100;

                int short_sleep;
                int long_sleep;
                int inter_sleep;

                logger.LogInformation("Heartbeat service started");

                // Signal that the background task has started
                await Utils.CancellableDelay(1000, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Global.IsDevelopment)
                        {
                            await Utils.CancellableDelay(100, stoppingToken);
                        }
                        else
                        {
                            var cpu = SystemStats.GetCpuUsage(1);
                            if (cpu == null)
                            {
                                short_sleep = short_sleep_max;
                            }
                            else
                            {
                                short_sleep = (int)(short_sleep_min + (short_sleep_max - short_sleep_min) * cpu.idlePercentage / 100);
                            }

                            long_sleep = 2 * short_sleep;
                            inter_sleep = 7 * short_sleep;

                            ioLib.heartbeat(1);
                            Thread.Sleep(short_sleep);
                            ioLib.heartbeat(0);
                            Thread.Sleep(long_sleep);
                            ioLib.heartbeat(1);
                            Thread.Sleep(short_sleep);
                            ioLib.heartbeat(0);
                            await Utils.CancellableDelay(inter_sleep, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error in heartbeat service: {ex}");
                        await Utils.CancellableDelay(1000, stoppingToken);
                    }
                }

                logger.LogInformation("Heartbeat service stopped");
            }
        }

    }

}
