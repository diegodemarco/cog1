using cog1.Business;
using cog1.DTO;
using cog1.Exceptions;
using cog1.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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
                    throw new ControllerException(Context.ErrorCodes.Users.INVALID_LOGIN_DETAILS);

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

        [HttpGet]
        [Route("users")]
        public List<UserDTO> EnumerateUsers()
        {
            return MethodPattern(() =>
            {
                if (Context.User.isAdmin)
                    // Admins can enumerate all users
                    return Context.UserBusiness.EnumerateUsers();
                return new()
                {
                    // Everyone else can only enumerate themselves
                    Context.UserBusiness.GetUser(Context.User.userId)
                };
            });
        }

        [HttpGet]
        [Route("users/{userId:int}")]
        public UserDTO GetUser(int userId)
        {
            return MethodPattern(() =>
            {
                if (Context.User.isAdmin || userId == Context.User.userId)
                    return Context.UserBusiness.GetUser(userId);
                throw new ControllerException(Context.ErrorCodes.Security.MUST_BE_ADMIN);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("users")]
        public UserDTO CreateUser([FromBody] UserWithPasswordDTO user)
        {
            return MethodPattern(() => Context.UserBusiness.CreateUser(user));
        }

        [HttpPut]
        [Route("users")]
        public UserDTO EditUser([FromBody] UserWithPasswordDTO user)
        {
            return MethodPattern(() =>
            {
                if (Context.User.isAdmin || user.user.userId == Context.User.userId)
                    return Context.UserBusiness.EditUser(user);
                throw new ControllerException(Context.ErrorCodes.Security.MUST_BE_ADMIN);
            });
        }

        [HttpDelete]
        [RequiresAdmin]
        [Route("users/{userId:int}")]
        public void DeleteUser(int userId)
        {
            MethodPattern(() => Context.UserBusiness.DeleteUser(userId));
        }

        [HttpPost]
        [Route("users/profile")]
        public UserDTO UpdateUserProfile([FromBody] UpdateProfileRequestDTO userProfile)
        {
            return MethodPattern(() =>
            {
                Context.UserBusiness.UpdateUserProfile(Context.User.userId, userProfile.localeCode);
                return Context.UserBusiness.GetUser(Context.User.userId);
            });
        }
    }
}
