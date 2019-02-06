using Dvelop.Remote;
using Dvelop.Selfhosted.HostApplication.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Selfhosted.HostApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost().Run();
        }

        public static IWebHost BuildWebHost()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .ConfigureServices(sc => sc
                    .AddSingleton<ICustomServiceProviderFactory>(new SelfHostedServiceProviderFactory())
                ).ConfigureAppConfiguration((context, builder) =>
                {
                    builder.Sources.Clear();
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    builder.AddEnvironmentVariables();
                } )
                .UseStartup<Startup>()
                .UseUrls("http://*:5000");

            var webHost = webHostBuilder.Build();
            
            return webHost;
        }
    }
}
