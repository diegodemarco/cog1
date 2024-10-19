using cog1.Business;
using cog1.DTO;
using cog1.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/security")]
    public class SecurityController : Cog1ControllerBase
    {
        private readonly ILogger<SecurityController> logger;

        public SecurityController(ILogger<SecurityController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public LoginResponseDTO Login([FromBody] LoginRequestDTO request)
        {
            return MethodPattern(() =>
            {
                if (!Context.UserBusiness.ValidateUserCredentials(request.userName, request.password, out var user))
                    throw new ControllerException(Context.ErrorCodes.User.INVALID_LOGIN_DETAILS);

                var token = Context.SecurityBusiness.CreateAccessToken(user.userId);
                return new LoginResponseDTO()
                {
                    token = token.ToString("N")
                };
            });
        }

        [HttpGet]
        [Route("access-token")]
        public AccessTokenInfoDTO GetAccessTokenInfo()
        {
            return MethodPattern(() =>
            {
                return new AccessTokenInfoDTO()
                {
                    user = Context.User
                };
            });
        }

    }
}
