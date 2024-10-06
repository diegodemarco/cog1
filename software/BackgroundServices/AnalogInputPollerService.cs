using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace cog1.Hardware
{
    public static partial class IOManager
    {

        /// <summary>
        /// Analog input poller lives inside the IOManager singleton, to periodically
        /// refresh analog inputs and have a "shadow" copy of their latest value.
        /// This class is nested inside the IOManager to have access to private 
        /// IOManager fields and methods.
        /// </summary>
        /// <param name="logger">logger used by the background service</param>
        public class AnalogInputPollerService(ILogger<AnalogInputPollerService> logger) : BackgroundService
        {

            protected async override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                logger.LogInformation("Analog polling service started");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        IOManager.AnalogRead();
                        await Utils.CancellableDelay(1000, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error in analog polling service: {ex}");
                        await Utils.CancellableDelay(1000, stoppingToken);
                    }
                }

                logger.LogInformation("Analog polling service stopped");
            }

        }

    }

}
