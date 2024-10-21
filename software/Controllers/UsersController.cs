using cog1.Business;
using cog1.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : Cog1ControllerBase
    {
        private readonly ILogger<LiteralsController> logger;

        public UsersController(ILogger<LiteralsController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpPost]
        [Route("profile")]
        public UserDTO UpdateProfile([FromBody] UpdateProfileRequestDTO userProfile)
        {
            return MethodPattern(() =>
            {
                Context.UserBusiness.UpdateUserProfile(Context.User.userId, userProfile.localeCode);
                return Context.UserBusiness.GetUser(Context.User.userId);
            });
        }

    }
}
