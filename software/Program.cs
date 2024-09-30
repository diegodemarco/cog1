using cog1.Display.Menu;
using cog1.Hardware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;

namespace cog1
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Startup.CreateSettings();

            try
            {
                var host = CreateHostBuilder(args).Build();
                var life = host.Services.GetRequiredService<IHostApplicationLifetime>();
                life.ApplicationStopped.Register(() =>
                {
                    // Deinitialize hardware i/o
                    IOManager.Deinit();
                });
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://0.0.0.0:80");
                });
        }

    }
}
