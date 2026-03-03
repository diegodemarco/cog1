using cog1.Business;
using cog1.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{

    /// <summary>
    /// The housekeeping background service takes care of running startup
    /// fixes on app startup, as well as periodically running housekeeping
    /// tasks that may be required by the various business layers.
    /// </summary>
    /// <param name="logger">
    /// Logger used by the background service
    /// </param>
    /// <param name="scopeFactory">
    /// Scope factory used to create new scopes as needed,
    /// mostly to instantiate contexts to work on startup fixes and periodic
    /// housekeeping tasks
    /// </param>
    public class HousekeepingService(ILogger<HousekeepingService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "Housekeeping", LogCategory.System)
    {

        protected override async Task Run(CancellationToken stoppingToken)
        {
            DoStartupFixes();

            // Postpone the first housekeeping for 15 seconds
            await Task.Yield();
            Utils.CancellableDelay(15000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    DoHousekeeping();

                    // Do housekeeing every 60 minutes
                    Utils.CancellableDelay(60 * 60 * 1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in housekeeping service: {ex}");
                    Utils.CancellableDelay(30000, stoppingToken);
                }
            }
        }

        private void DoStartupFixes()
        {
            logger.LogInformation("Started startup fixes");
            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<Cog1Context>())
                {
                    foreach (var business in context.EnumerateBusinessObjects())
                        business.DoStartupFixes();
                    context.Commit();
                }
            }
            logger.LogInformation("Completed startup fixes");
        }

        private void DoHousekeeping()
        {
            logger.LogInformation("Starting housekeeping");
            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<Cog1Context>())
                {
                    foreach (var business in context.EnumerateBusinessObjects())
                        business.DoHousekeeping();
                    context.Commit();
                }
            }
            logger.LogInformation("Completed housekeeping");
        }

    }

}
