using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace cog1app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            // this will serve js, css, images etc.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "admin")),
                RequestPath = "/admin"
            });

            app.UseRouting();

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
