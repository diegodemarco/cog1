using cog1.Business;
using cog1.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/entities")]
    public class EntitiesController : Cog1ControllerBase
    {
        private readonly ILogger<EntitiesController> logger;

        public EntitiesController(ILogger<EntitiesController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("basic")]
        public BasicEntitiesContainerDTO GetBasicEntities()
        {
            return MethodPattern(() =>
            {
                return new BasicEntitiesContainerDTO()
                {
                    literals = Context.Literals,
                    locales = Context.MasterEntityBusiness.EnumerateLocales(),
                    variableTypes = Context.VariableBusiness.EnumerateVariableTypes(),
                    variableDirections = Context.VariableBusiness.EnumerateVariableDirections(),
                };
            });
        }

    }
}
