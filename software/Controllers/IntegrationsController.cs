using cog1.Business;
using cog1.DTO;
using cog1.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/integrations")]
    public class IntegrationsController : Cog1ControllerBase
    {
        private readonly ILogger<IntegrationsController> logger;

        public IntegrationsController(ILogger<IntegrationsController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        // Integration connections

        [HttpGet]
        [Route("connections")]
        public List<IntegrationConnectionDTO> EnumerateConnections()
        {
            return MethodPattern(() => Context.IntegrationBusiness.EnumerateIntegrationConnections()
                .OrderBy(item => item.integrationConnectionId)
                .ToList());
        }

        [HttpGet]
        [Route("connections/{id:int}")]
        public IntegrationConnectionDTO GetConnectionById(int id)
        {
            return MethodPattern(() => Context.IntegrationBusiness.GetIntegrationConnection(id));
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("connections")]
        public IntegrationConnectionDTO CreateConnection([FromBody] IntegrationConnectionDTO dto)
        {
            return MethodPattern(() => Context.IntegrationBusiness.CreateIntegrationConnection(dto));
        }

        [HttpPut]
        [RequiresAdmin]
        [Route("connections")]
        public IntegrationConnectionDTO EditConnection([FromBody] IntegrationConnectionDTO dto)
        {
            return MethodPattern(() => Context.IntegrationBusiness.EditIntegrationConnection(dto));
        }

        [HttpDelete]
        [RequiresAdmin]
        [Route("connections/{id:int}")]
        public void DeleteConnection(int id)
        {
            MethodPattern(() =>
            {
                Context.IntegrationBusiness.DeleteIntegrationConnection(id);
            });
        }

        // Outbound integrations

        [HttpGet]
        [Route("outbound")]
        public List<OutboundIntegrationDTO> EnumerateOutboundIntegrations()
        {
            return MethodPattern(() => Context.IntegrationBusiness.EnumerateOutboundIntegrations()
                .OrderBy(item => item.integrationId)
                .ToList());
        }

        [HttpGet]
        [Route("outbound/{id:int}")]
        public OutboundIntegrationDTO GetOutboundIntegrationById(int id)
        {
            return MethodPattern(() => Context.IntegrationBusiness.GetOutboundIntegration(id));
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("outbound")]
        public OutboundIntegrationDTO CreateOutboundIntegration([FromBody] OutboundIntegrationDTO dto)
        {
            return MethodPattern(() => Context.IntegrationBusiness.CreateOutboundIntegration(dto));
        }

        [HttpPut]
        [RequiresAdmin]
        [Route("outbound")]
        public OutboundIntegrationDTO EditOutboundIntegration([FromBody] OutboundIntegrationDTO dto)
        {
            return MethodPattern(() => Context.IntegrationBusiness.EditOutboundIntegration(dto));
        }

        [HttpDelete]
        [RequiresAdmin]
        [Route("outbound/{id:int}")]
        public void DeleteOutboundIntegration(int id)
        {
            MethodPattern(() =>
            {
                Context.IntegrationBusiness.DeleteOutboundIntegration(id);
            });
        }
    }
}
