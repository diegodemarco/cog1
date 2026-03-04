using cog1.Business;
using cog1.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{
    /// <summary>
    /// Base class for all background services. Stores a service name and log category,
    /// and delegates execution to the abstract <see cref="Run"/> method.
    /// </summary>
    public abstract class BaseBackgroundService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly string serviceName;
        private readonly LogCategory logCategory;
        protected readonly IServiceScopeFactory scopeFactory;

        protected ILogger Logger => logger;
        protected string ServiceName => serviceName;
        protected LogCategory LogCategory => logCategory;

        protected BaseBackgroundService(ILogger logger, IServiceScopeFactory scopeFactory, string serviceName, LogCategory logCategory)
        {
            this.logger = logger;
            this.serviceName = serviceName;
            this.logCategory = logCategory;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogInformation($"{ServiceName} service started");

            await Task.Run(() => Run(stoppingToken));

            LogInformation($"{ServiceName} service stopped");
        }

        protected void LogInformation(string text)
        {
            logger.LogInformation(text);
            LoggingBusiness.Log(LogCategory, DTO.LogLevel.Information, text);
        }

        protected void LogWarning(string text)
        {
            logger.LogWarning(text);
            LoggingBusiness.Log(LogCategory, DTO.LogLevel.Warning, text);
        }

        protected void LogError(string text)
        {
            logger.LogError(text);
            LoggingBusiness.Log(LogCategory, DTO.LogLevel.Error, text);
        }

        /// <summary>
        /// Implement this method in descendant classes to define the background service's
        /// main execution logic.
        /// </summary>
        protected abstract Task Run(CancellationToken stoppingToken);
    }
}
