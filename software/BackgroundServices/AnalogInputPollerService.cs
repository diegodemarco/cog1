using cog1.BackgroundServices;
using cog1.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace cog1.System
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
        public class AnalogInputPollerService(ILogger<AnalogInputPollerService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "Analog input poller", LogCategory.Variables)
        {

            protected async override Task Run(CancellationToken stoppingToken)
            {
                await Task.Yield();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        IOManager.AnalogRead();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error in analog polling service: {ex}");
                    }
                    Utils.CancellableDelay(1000, stoppingToken);
                }
            }

        }

    }

}
