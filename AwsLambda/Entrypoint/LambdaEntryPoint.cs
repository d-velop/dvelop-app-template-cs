using System;
using Amazon.Lambda.Core;
using Dvelop.Lambda.EntryPoint.DependencyInjection;
using Dvelop.Remote;
using Dvelop.Sdk.Logging.OtelJsonConsole.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Dvelop.Lambda.EntryPoint
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// EntryPoint::Dvelop.Lambda.EntryPoint.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint : 
        
        // The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
        // will fail to convert the incoming request correctly into a valid ASP.NET Core request.
        //
        // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
        // Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        // 
        // Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
        // will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.
        Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
    {
        /// <summary>
        /// 
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<Startup>() method.
        /// </summary>
        /// <param name="webHostBuilder"></param>
        protected override void Init(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureWebHostDefaults(webHostBuilder =>
            {
                webHostBuilder
                    .ConfigureAppConfiguration((context, configurationBuilder) =>
                        {
                            configurationBuilder.Sources.Clear();
                            configurationBuilder.AddEnvironmentVariables();
                        }
                    )
                    .ConfigureLogging((hostingContext, loggingBuilder) =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddOtelJsonConsole(c => c.IncludeScopes=false);
                        loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                        loggingBuilder.AddFilter("Default", LogLevel.Debug);
                        loggingBuilder.AddFilter("System", LogLevel.Information);
                        loggingBuilder.AddFilter("Microsoft", LogLevel.Information);

                    })
                    .UseStartup<Startup>();
            }).UseServiceProviderFactory(new EnvironmentAwareServiceProviderFactory(new AwsServiceProviderFactory()));;
        }
    }

}
