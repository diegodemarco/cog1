using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace cog1app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (IOManager.Init())
            {
                SystemStats.Init();
                DisplayMenu.Init();
                BackgroundChecker.Init();

                var host = CreateHostBuilder(args).Build();
                var life = host.Services.GetRequiredService<IHostApplicationLifetime>();
                life.ApplicationStopped.Register(() =>
                {
                    IOManager.Deinit();
                    BackgroundChecker.Deinit();
                });
                host.Run();
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
