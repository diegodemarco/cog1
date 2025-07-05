using cog1.Business;
using cog1.DTO;
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

        [HttpPost]
        [RequiresAdmin]
        [Route("wifi/connect")]
        public void WiFiConnect([FromBody] WiFiConnectRequestDTO request)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Connect(request.ssid, request.password))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_CONNECT_ERROR);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("wifi/reconnect")]
        public void WiFiReconnect([FromBody] WiFiSsidRequestDTO request)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Reconnect(request.ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_CONNECT_ERROR);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("wifi/disconnect")]
        public void WiFiDisconnect([FromBody] WiFiSsidRequestDTO request)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Disconnect(request.ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_DISCONNECT_ERROR);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("wifi/forget")]
        public void WiFiForget([FromBody] WiFiSsidRequestDTO request)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.Forget(request.ssid))
                    throw new ControllerException(context.ErrorCodes.Network.WIFI_FORGET_ERROR);
            });
        }

        [HttpGet]
        [Route("wifi/ip-configuration")]
        public IpConfigurationDTO WiFiGetIpConfiguration([FromQuery] string ssid)
        {
            return MethodPattern(() =>
            {
                var result = WiFiManager.GetIpConfiguration(ssid);
                if (result == null)
                    throw new ControllerException(context.ErrorCodes.Network.IP_CONFIG_READ_ERROR);
                return result;
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("wifi/ip-configuration")]
        public void WiFiSetIpConfiguration([FromBody] WiFiSetIpConfigurationDTO request)
        {
            MethodPattern(() =>
            {
                if (!WiFiManager.SetIpConfiguration(request.ssid, request.ipConfiguration))
                    throw new ControllerException(context.ErrorCodes.Network.IP_CONFIG_ERROR);
            });
        }

        #endregion

        #region Ethernet

        [HttpGet]
        [Route("ethernet/ip-configuration")]
        public IpConfigurationDTO EthernetGetIpConfiguration()
        {
            return MethodPattern(() =>
            {
                var result = EthernetManager.GetIpConfiguration();
                if (result == null)
                    throw new ControllerException(context.ErrorCodes.Network.IP_CONFIG_READ_ERROR);
                return result;
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("ethernet/ip-configuration")]
        public void EthernetSetIpConfiguration([FromBody] IpConfigurationDTO configuration)
        {
            MethodPattern(() =>
            {
                if (!EthernetManager.SetIpConfiguration(configuration))
                    throw new ControllerException(context.ErrorCodes.Network.IP_CONFIG_ERROR);
            });
        }

        [HttpGet]
        [Route("ethernet/link-configuration")]
        public EthernetLinkConfigurationDTO EthernetGetLinkConfiguration()
        {
            return MethodPattern(() =>
            {
                var result = EthernetManager.GetLinkConfiguration();
                if (result == null)
                    throw new ControllerException(context.ErrorCodes.Network.LINK_CONFIG_READ_ERROR);
                return result;
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("ethernet/link-configuration")]
        public void EthernetSetLinkConfiguration([FromBody] EthernetLinkConfigurationDTO configuration)
        {
            MethodPattern(() =>
            {
                if (!EthernetManager.SetLinkConfiguration(configuration, context.ErrorCodes))
                    throw new ControllerException(context.ErrorCodes.Network.LINK_CONFIG_ERROR);
            });
        }

        #endregion
    }
}
