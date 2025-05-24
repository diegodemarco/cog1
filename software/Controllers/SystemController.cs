using cog1.Business;
using cog1.Telemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/system")]
    public class SystemController : Cog1ControllerBase
    {
        private readonly ILogger<SystemController> logger;
        private readonly Cog1Context context;

        public SystemController(ILogger<SystemController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        [Route("stats")]
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

        [HttpGet]
        [Route("stats/cpu/history-5min")]
        public List<double> getCpuHistory5Min()
        {
            return MethodPattern(() =>
            {
                try
                {
                    return SystemStats.GetCPUHistory(5 * 60);
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
