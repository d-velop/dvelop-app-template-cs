using Dvelop.Remote;
using Dvelop.Sdk.Logging.OtelJsonConsole.Extension;
using Dvelop.Selfhosted.HostApplication.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Dvelop.Selfhosted.HostApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost().Run();
        }

        public static IHost BuildWebHost()
        {
            var hostBuilder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((context, configBuilder) =>
                    {
                        configBuilder.AddJsonFile("appsettings.json", true, true);
                        configBuilder.AddJsonFile("appsettings.custom.json", true, true);
                        configBuilder.AddEnvironmentVariables();
                    })
                    .ConfigureLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSimpleConsole(options=>
                        {
                            options.SingleLine = true;
                            options.ColorBehavior = LoggerColorBehavior.Disabled;
                        });
                        // loggingBuilder.AddOtelJsonConsole();
                    })
                    .UseStartup<Startup>();
            }).UseServiceProviderFactory(new EnvironmentAwareServiceProviderFactory(new SelfHostedServiceProviderFactory()));

            var webHost = hostBuilder.Build();

            return webHost;

        }
    }
}
