using cog1.Business;
using cog1.Exceptions;
using cog1.Hardware;
using cog1.Middleware;
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

        #region Wi-Fi

        [HttpGet]
        [Route("wifi/scan")]
        public List<WiFiSsidDTO> getWiFiScan()
        {
            return MethodPattern(() =>
            {
                try
                {
                    return WiFiManager.Scan();
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText("err.txt", ex.ToString());
                    throw;
                }
            });
        }

        [HttpGet]
        [RequiresAdmin]
        [Route("wifi/connect")]
        public void WiFiConnect([FromQuery] string ssid, [FromQuery] string password)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Connect(ssid, password))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_CONNECT_ERROR);
            });
        }

        [HttpGet]
        [RequiresAdmin]
        [Route("wifi/reconnect")]
        public void WiFiReconnect([FromQuery] string ssid)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Reconnect(ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_CONNECT_ERROR);
            });
        }

        [HttpGet]
        [RequiresAdmin]
        [Route("wifi/disconnect")]
        public void WiFiDisconnect([FromQuery] string ssid)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Disconnect(ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_DISCONNECT_ERROR);
            });
        }

        [HttpGet]
        [RequiresAdmin]
        [Route("wifi/forget")]
        public void WiFiForget([FromQuery] string ssid)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Forget(ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_FORGET_ERROR);
            });
        }

        #endregion
    }
}
