using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace cog1app.Controllers
{
    [ApiController]
    [Route("api")]
    public class Cog1APIController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Cog1APIController> _logger;

        public Cog1APIController(ILogger<Cog1APIController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return DateTime.UtcNow.ToString("s");
        }

        [HttpGet]
        [Route("systemstats")]
        public SystemStatsReport GetSystemStats()
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
        }
    }
}
