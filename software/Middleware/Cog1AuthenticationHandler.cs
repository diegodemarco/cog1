using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;
using cog1.Business;
using cog1.Exceptions;
using cog1.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace cog1.Middleware
{
    public class Cog1AuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class Cog1AuthenticationHandler : AuthenticationHandler<Cog1AuthenticationOptions>
    {
        private readonly Cog1Context context;

        public Cog1AuthenticationHandler(IOptionsMonitor<Cog1AuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, Cog1Context context) : base(options, logger, encoder)
        {
            this.context = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Allow "OPTIONS" requests
            if (string.Equals(Request.Method, "options", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.NoResult());

            // If this is a swagger request, allow it
            if (Request.Path == "/" || Request.Path.StartsWithSegments("/swagger") || Request.Path.StartsWithSegments("/console"))
                return Task.FromResult(AuthenticateResult.NoResult());

            // If anonymous access is allowed, go ahead
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return Task.FromResult(AuthenticateResult.NoResult());

            var accessToken = Request.Query["accessToken"];
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    Console.WriteLine("No authorization header");
                    throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
                }

                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    Console.WriteLine("No authorization value");
                    throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
                }

                if (!authorizationHeader.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Authorization does not start with 'bearer'");
                    throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
                }

                // Validate token
                accessToken = authorizationHeader.Substring("bearer ".Length).Trim();
            }

            if (!Guid.TryParse(accessToken, out Guid tokenGuid))
            {
                Console.WriteLine("The token is not a guid");
                throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
            }

            if (!context.SecurityBusiness.ValidateAccessToken(tokenGuid, out UserDTO user))
            {
                Console.WriteLine("ValidateAccessToken() rejected the token");
                throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
            }

            // Claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.userName));
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            if (user.isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            // Create ticket
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            // Done
            context.SetUser(user);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

    }

}
