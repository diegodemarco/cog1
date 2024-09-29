using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using cog1.Business;
using cog1.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;

namespace cog1.Middleware
{
    public class Cog1Middleware
    {
        private readonly RequestDelegate nextDelegate;
        private readonly IConfiguration configuration;
        private readonly ILogger<Cog1Middleware> logger;

        public Cog1Middleware(RequestDelegate next, IConfiguration configuration, ILogger<Cog1Middleware> logger)
        {
            this.nextDelegate = next;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, Cog1Context context)
        {
            try
            {
                HttpRequestRewindExtensions.EnableBuffering(httpContext.Request, 1024 * 64);        // 64K threshold
                await nextDelegate(httpContext);
                var rsp = httpContext.Response;
                var mustCommit = !context.Committed;
                if (!httpContext.Response.HasStarted)
                {
                    // Handling of authorization and authentication errors
                    if (rsp.StatusCode == 401 || rsp.StatusCode == 403)
                        throw new ControllerException(context.ErrorCodes.Security.INVALID_ACCESS_TOKEN);
                }
                if (rsp.StatusCode >= 400)
                {
                    // Handling of all other error codes that did not result in an exception, but
                    // still should roll the current transaction back.
                    mustCommit = false;
                }
                // Auto commit
                if (mustCommit)
                    context.Commit();
                (context as IDisposable).Dispose();
            }
            catch (Exception e)
            {
                ControllerException se = (e is ControllerException) ? (ControllerException)e : new ControllerException(e);
                LogException(httpContext, se);
                if (!httpContext.Response.HasStarted)
                {
                    httpContext.Response.ContentType = "application/json; charset=utf-8";
                    if (httpContext.Response.StatusCode < 400)
                        httpContext.Response.StatusCode = (int)se.StatusCode;
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(se.ToJson()));
                }
            }
        }

        private void LogException(HttpContext httpContext, ControllerException se)
        {
            StringBuilder SB = new StringBuilder();
            if (!string.IsNullOrEmpty(se.ClassName))
                SB.Append(se.ClassName);
            if (!string.IsNullOrEmpty(se.Message))
            {
                if (SB.Length > 0) SB.Append("; ");
                SB.Append(se.Message);
            }
            if (!string.IsNullOrEmpty(se.FaultCode))
            {
                if (SB.Length > 0) SB.Append("; ");
                SB.Append("FaultCode: " + se.FaultCode);
            }
            if (!string.IsNullOrEmpty(se.FaultData))
            {
                if (SB.Length > 0) SB.Append("; ");
                SB.Append("FaultData: " + se.FaultData);
            }
            var uri = new Uri(httpContext.Request.GetDisplayUrl()).PathAndQuery;
            logger.LogError($"{httpContext.Request.Method} {uri} - {SB.ToString().TrimEnd()}");
        }

        public class JSonErrorResponse : JsonResult
        {

            public JSonErrorResponse(ErrorCode e) : base(new ControllerException(e).ToJson()) { }

            public JSonErrorResponse(ErrorCode e, string extraData) : base(new ControllerException(e, extraData).ToJson()) { }

            public JSonErrorResponse(Exception e) : base(new ControllerException(e).ToJson()) { }

        }

    }

}
