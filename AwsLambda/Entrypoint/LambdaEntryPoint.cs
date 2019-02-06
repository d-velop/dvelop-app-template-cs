using Dvelop.Lambda.EntryPoint.DependencyInjection;
using Dvelop.Remote;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                .UseStartup<Startup>();
        }
    }
}
