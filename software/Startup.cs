using cog1.BackgroundServices;
using cog1.Business;
using cog1.Display.Menu;
using cog1.Exceptions;
using cog1.Hardware;
using cog1.Literals;
using cog1.Middleware;
using cog1.Telemetry;
using Cog1.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace cog1
{
    public class Startup()
    {

        public static void CreateSettings()
        {
            // Create a default "appsettings.json" if it doesn't exist.
            const string cfg_file = "appsettings.json";
            if (!File.Exists(cfg_file))
            {
                var dict = new Dictionary<string, object>()
                {
                    {
                        "Logging", new Dictionary<string, object>()
                        {
                            {
                                "LogLevel", new Dictionary<string, string>()
                                {
                                    { "Default", "Information" },
                                    { "Microsoft", "Warning" },
                                    { "Microsoft.Hosting.Lifetime", "Information" }
                                }
                            },
                        }
                    },
                    {
                        "AllowedHosts", "*"
                    }
                };
                File.WriteAllText(cfg_file, JsonConvert.SerializeObject(dict, Formatting.Indented));
            }

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Initialize database
            Cog1DbContext.InitializeDatabase();

            // Initialize hardware i/o
            if (!IOManager.Init())
                throw new System.Exception("Failed startup: could not initialize hardware");

            // HttpContext is used by the cog1 context
            services.AddHttpContextAccessor();

            // Use the cog1 context
            services.AddScoped<Cog1Context>();

            // Add custom authentication
            services.AddAuthentication("cog1")
                .AddScheme<Cog1AuthenticationOptions, Cog1AuthenticationHandler>("cog1", null);

            // Register background services
            services.AddHostedService<IOManager.HeartbeatService>();
            services.AddHostedService<IOManager.AnalogInputPollerService>();
            services.AddHostedService<SystemStats.BackgroundTelemetryService>();
            services.AddHostedService<WiFiManager.WiFiMonitorService>();
            services.AddHostedService<DisplayMenu.MenuLoopService>();
            services.AddHostedService<HousekeepingService>();

            // Add API controllers
            services.AddControllers();

            // Add swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("current", new OpenApiInfo { Title = "cog1 API", Version = "v1" });
                c.SchemaFilter<EnumSchemaFilter>();
                c.OperationFilter<EndpointErrorsFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configure CORS
            app.UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            // this will serve js, css, images etc.
            var consolePath = Path.Combine(Directory.GetCurrentDirectory(), "console");
            if (!Directory.Exists(consolePath))
                Directory.CreateDirectory(consolePath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(consolePath),
                RequestPath = "/console"
            });

            app.UseRouting();

            // The cog1 middleware will render exceptions correctly, returning them in a
            // standard json format to the front-end.
            app.UseMiddleware<Cog1Middleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // this ensures index.html is served for any requests with a path
            // and prevents a 404 when the user refreshes the browser
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && context.Request.Path.StartsWithSegments("/console"))
                {
                    context.Response.ContentType = "text/html";

                    await context.Response.SendFileAsync(
                        env.ContentRootFileProvider.GetFileInfo("console/index.html")
                    );

                    return;
                }
                else if (context.Request.Path.HasValue && context.Request.Path.Value == "/")
                {
                    context.Response.Redirect("/console");

                    return;
                }
                await next();
            });

            // Enable swagger
            app.UseSwagger();

        }

        #region Swagger fixes

        internal sealed class EndpointErrorsFilter : IOperationFilter
        {
            private static object _lock = new();
            private static HashSet<HttpStatusCode> usedErrorCodes = null;

            private void LoadErrorCodes()
            {
                lock (_lock)
                {
                    if (usedErrorCodes != null)
                        return;

                    usedErrorCodes = new();
                    var errorCodes = new Exceptions.ErrorCodes(Locales.English.LocaleCode);
                    foreach (var prop in errorCodes.GetType().GetProperties().Where(item => item.PropertyType == typeof(ErrorCodesContainer)
                      || item.PropertyType.IsSubclassOf(typeof(ErrorCodesContainer))))
                    {
                        var container = prop.GetValue(errorCodes) as ErrorCodesContainer;
                        foreach (var err in prop.PropertyType.GetProperties().Where(item => item.PropertyType == typeof(ErrorCode)
                          || item.PropertyType.IsSubclassOf(typeof(ErrorCode))))
                        {
                            var errorCode = err.GetValue(container) as ErrorCode;
                            //Console.WriteLine($"{prop.Name}.{err.Name}={errorCode.StatusCode}");
                            usedErrorCodes.Add(errorCode.StatusCode);
                        }
                    }
                }
            }

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                LoadErrorCodes();

                // Check if JsonControllerException is already in the schema
                context.SchemaRepository.TryLookupByType(typeof(ControllerException.JsonControllerException), out var schema);
                if (schema is null)
                    schema = context.SchemaGenerator.GenerateSchema(typeof(ControllerException.JsonControllerException), context.SchemaRepository);

                var response200 = operation.Responses["200"];
                if (response200.Content.Count < 1)
                {
                    context.SchemaRepository.TryLookupByType(typeof(object), out var objectSchema);
                    if (objectSchema == null)
                        objectSchema = context.SchemaGenerator.GenerateSchema(typeof(object), context.SchemaRepository);
                    response200.Content.Add("text/plain", new OpenApiMediaType { Schema = objectSchema });
                    response200.Content.Add("application/json", new OpenApiMediaType { Schema = objectSchema });
                    response200.Content.Add("text/json", new OpenApiMediaType { Schema = objectSchema });
                }

                var content = new Dictionary<string, OpenApiMediaType>()
                {
                    { "text/json", new OpenApiMediaType { Schema = schema } }
                };
                foreach (var errorCode in usedErrorCodes)
                {
                    operation.Responses.Add(((int)errorCode).ToString(), new OpenApiResponse() { Content = content, Description = $"{errorCode}" });
                }
            }
        }

        /// <summary>
        /// This filter is used to ensure that swagger when rendering swagger.json, 
        /// enums are represented with their values, but also their names are included,
        /// so that code-generation tools (like swagger-typescript-api, used by the
        /// frontend), can generate enums that contain the proper names.
        /// </summary>
        internal sealed class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema model, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    var xEnumNames = new OpenApiArray();
                    xEnumNames.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
                    model.Extensions.Add("x-enumNames", xEnumNames);
                }
            }
        }

        #endregion
    }
}
