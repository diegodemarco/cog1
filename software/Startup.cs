using cog1.Business;
using cog1.Display.Menu;
using cog1.Hardware;
using cog1.Middleware;
using Cog1.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace cog1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void CreateSettings()
        {
            // Check appsettings 
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
            // HttpContext is used by the cog1 context
            services.AddHttpContextAccessor();

            // Use the cog1 context
            services.AddScoped<Cog1Context>();

            // Add custom authentication
            services.AddAuthentication("cog1")
                .AddScheme<Cog1AuthenticationOptions, Cog1AuthenticationHandler>("cog1", null);

            // Add API controllers
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Global.IsDevelopment = env.IsDevelopment();

            // Initialize database
            Cog1DbContext.InitializeDatabase();

            if (IOManager.Init())
            {
                SystemStats.Init();
                WiFiManager.Init();
                DisplayMenu.Init();
            }
            else
            {
                throw new System.Exception("Failed startup: could not initialize hardware");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            // this will serve js, css, images etc.
            var adminPath = Path.Combine(Directory.GetCurrentDirectory(), "admin");
            if (!Directory.Exists(adminPath))
                Directory.CreateDirectory(adminPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(adminPath),
                RequestPath = "/admin"
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
                if (context.Request.Path.HasValue && context.Request.Path.StartsWithSegments("/admin"))
                {
                    context.Response.ContentType = "text/html";

                    await context.Response.SendFileAsync(
                        env.ContentRootFileProvider.GetFileInfo("admin/index.html")
                    );

                    return;
                }
                else if (context.Request.Path.HasValue && context.Request.Path.Value == "/")
                {
                    context.Response.Redirect("/admin");

                    return;
                }
                await next();
            });

        }
    }
}
