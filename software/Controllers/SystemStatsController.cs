using cog1.Business;
using cog1.Telemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/systemstats")]
    public class SystemStatsController : Cog1ControllerBase
    {
        private readonly ILogger<SystemStatsController> logger;
        private readonly Cog1Context context;

        public SystemStatsController(ILogger<SystemStatsController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public SystemStatsReport GetSystemStats()
        {
            return MethodPattern(() =>
            {
                try
                {
                    return SystemStats.GetReport();
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText("err.txt", ex.ToString());
                    throw;
                }
            });
        }
    }
}
