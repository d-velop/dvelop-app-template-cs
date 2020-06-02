using System;
using Amazon.Lambda.Core;
using Dvelop.Lambda.EntryPoint.DependencyInjection;
using Dvelop.Remote;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.LambdaJsonSerializer))]
namespace Dvelop.Lambda.EntryPoint
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// EntryPoint::Dvelop.Lambda.EntryPoint.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="webHostBuilder"></param>
        protected override void Init(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder                
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                    {
                        configurationBuilder.Sources.Clear();
                        configurationBuilder.AddEnvironmentVariables();
                    }
                )
                .ConfigureServices( sc => 
                    sc.AddSingleton<ICustomServiceProviderFactory>(new AwsServiceProviderFactory()) 
                )
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    /*
                    logging.AddConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.DisableColors = true;
                        options.Format = ConsoleLoggerFormat.Default;
                    });
                    */
                        logging.AddLambdaLogger(new LambdaLoggerOptions
                        {
                            IncludeLogLevel = true,
                            Filter = (category, logLevel) => true,
                            IncludeException = true,
                            IncludeNewline = false,
                            IncludeEventId = false,
                            IncludeCategory = true
                        });
                     
                })
                .UseStartup<Startup>();
        }
    }
}
