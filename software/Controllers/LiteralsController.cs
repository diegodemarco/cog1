using cog1.Business;
using cog1.Literals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/literals")]
    public class LiteralsController : Cog1ControllerBase
    {
        private readonly ILogger<LiteralsController> logger;

        public LiteralsController(ILogger<LiteralsController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public LiteralsContainerDTO GetLiterals()
        {
            return MethodPattern(() =>
            {
                return Context.Literals;
            });
        }

    }
}
