using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Dvelop.Sdk.TenantMiddleware;
using Dvelop.Domain.Repositories;
using Dvelop.Remote.Constraints;
using Dvelop.Remote.Filter;
using Dvelop.Sdk.IdentityProvider.Client;
using Dvelop.Sdk.IdentityProvider.Middleware;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Dvelop.Remote
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        private readonly ICustomServiceProviderFactory _factory;

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ICustomServiceProviderFactory factory, ILoggerFactory loggerFactory)
        {
            _factory = factory;
            
            _logger = loggerFactory.CreateLogger<Startup>();
            
            Configuration = configuration;

            Configuration["DEFAULT_SYSTEM_BASE_URI"] = Configuration["SYSTEMBASEURI"]??"http://localhost";
            Configuration["SIGNATURE_SECRET"] = Configuration["SIGNATURE_SECRET"]??"";

            Configuration["APP_NAME"] = Configuration["APP_NAME"]??"acme-apptemplatecs";
            Configuration["BASE"] = $"/{Configuration["APP_NAME"]}";
            Configuration["ASSETS"] = Configuration["ASSET_BASE_PATH"]??$"{Configuration["DEFAULT_SYSTEM_BASE_URI"]}{Configuration["BASE"]}";
            
            _logger.LogInformation($"SYSTEMBASEURI: {Configuration["SYSTEMBASEURI"]}");
            _logger.LogInformation($"DEFAULT_SYSTEM_BASE_URI: {Configuration["DEFAULT_SYSTEM_BASE_URI"]}");
            _logger.LogInformation($"SIGNATURE_SECRET set: {!string.IsNullOrWhiteSpace(Configuration["SIGNATURE_SECRET"])}");
            _logger.LogInformation($"APP_NAME: {Configuration["APP_NAME"]}");
            _logger.LogInformation($"ASSETS: {Configuration["ASSETS"]}");
            _logger.LogInformation($"BASE: {Configuration["BASE"]}");
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            
            // Add Filter for DvSignature
            services.AddScoped<Dv1HmacSha256SignatureFilter>();
            
            // Add HttpClientService
            services.AddHttpClient();
            services.AddHttpClient("test");
            
            // Allow Classes to access the HttpContext
            services.AddHttpContextAccessor();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ProducesMatcherPolicy>());
            // Enable d.ecs IdentityProvider
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "IdentityProvider";
                options.DefaultChallengeScheme = "IdentityProvider";
                options.DefaultForbidScheme = "IdentityProvider";

            }).AddIdentityProviderAuthentication("IdentityProvider", "d.velop Identity Provider", options => {  });
            services.AddAuthorization(options =>
            {
                // DefaultPolicy will be evaluated, if there is an [Authorize] attribute, but no configuration.
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // FallbackPolicy will be evaluated, if there is neither [Authorize] nor an [AllowAnonymous] attribute provided.
                options.FallbackPolicy= new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
            // Create and configure Mvc/Razor
            services.AddRazorPages()
                .AddMvcOptions(options => options.RespectBrowserAcceptHeader = true)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AllowAnonymousToPage("/Error");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
                /* If you want to switch back to NewtonSoftJson, use to following settings
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })*/
                
            services.AddDirectoryBrowser();
            services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Information));
            services.AddRouting(routeOptions => routeOptions.AppendTrailingSlash = true );
            return _factory.CreateServiceProvider(services);
        }

        // This method gets called by the ASP .NET core runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider  actionDescriptorProvider)
        {
            // Print information about bound routes and the Controller, they are bound to.
            var routes = actionDescriptorProvider.ActionDescriptors.Items.Where(ad => ad.AttributeRouteInfo != null).ToList();
            routes.ForEach(ad =>
            {
                _logger.LogInformation($"{ad.AttributeRouteInfo.Template} -> '/{ad.AttributeRouteInfo.Name}'");
            });

            // Important:
            // If running without 'api_custom_domains'-Feature, the requests need to be rewritten to omit the
            // API-Gateway stage /prod or /dev, if running as a d.velop cloud App within a tenant aware environment.
            app.UseRewriter(new RewriteOptions()
                
                // Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction sets the BasePath-property to the name of the API-Gateway Stage.
                // This RewriteRule can or should be removed, if: 
                //   - The 'api_custom_domains'-Feature is activated
                //   - There is no url-Rewriting Reverse-Proxy (You call the Api-Gateway Url directly).
                .Add(rc =>
                    {
                        var oldPathBase = rc.HttpContext.Request.PathBase;
                        
                        // Set the RawTarget, because in an AWS Lambda it is not available (yet)
                        // See: https://github.com/aws/aws-lambda-dotnet/issues/656
                        var requestFeature = rc.HttpContext.Features.Get<IHttpRequestFeature>();
                        requestFeature.RawTarget ??= rc.HttpContext.Request.GetEncodedPathAndQuery();
                        
                        rc.HttpContext.Request.PathBase = "";
                        _logger.LogInformation($"Changed PathBase from '{oldPathBase}' to '{rc.HttpContext.Request.PathBase}'");
                        rc.Result = RuleResult.ContinueRules;
                    })

                // This redirect ensures, that a URL is always used with an trailing '/', expect in the last segment ist a '.'.
                // .AddRedirect(@"^(((.*/)|(/?))[^/.]+(?!/$))$", "$1/",302)
            );
            
            // This will a a virtual path-segment to the application
            app.UsePathBase(Configuration["BASE"]);
            
            // Enable Multi-Tenancy
            app.UseTenantMiddleware(new TenantMiddlewareOptions
            {
                DefaultSystemBaseUri = Configuration["DEFAULT_SYSTEM_BASE_URI"],
                DefaultTenantId = "0",
                SignatureSecretKey = Convert.FromBase64String(Configuration["SIGNATURE_SECRET"]),

                OnTenantIdentified = (tenantId, systemBaseUri) =>
                {
                    // Use Built-In Dependency Injection to Store Tenant Information (Bound to Request-Context)
                    var tenantRepository = app.ApplicationServices.GetService<ITenantRepository>();
                    tenantRepository.SystemBaseUri = new Uri(systemBaseUri);
                    tenantRepository.TenantId = tenantId;
                }
            });
            // Enable d.ecs IdentityProvider
            app.UseIdentityProvider(new IdentityProviderOptions
            {
                BaseAddress = new Uri(Configuration["DEFAULT_SYSTEM_BASE_URI"]),
                HttpClient = new HttpClient(),
                TenantInformationCallback = () =>
                {
                    var tenantRepository = app.ApplicationServices.GetService<ITenantRepository>();
                    _logger.LogError($"Tenant Identified {tenantRepository.TenantId} {tenantRepository.SystemBaseUri}");
                    return new TenantInformation
                    {
                        TenantId = tenantRepository.TenantId,
                        SystemBaseUri = tenantRepository.SystemBaseUri.ToString()
                    };
                    
                }
            });

            if (env.IsDevelopment())
            {
                // We want to see detailed information about errors
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // This is the Production Environment
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.Use(async (httpContext, next) =>
            {
                // Vary Header determines which additional header fields should be used
                // to decide if a request can be answered from a cache
                // cf. https://tools.ietf.org/html/rfc7234#section-4.1
                // accept is added because most resources deliver JSON and HTML from the same URI
                // x-dv-sig-1 ist added because most of the responses are tenant specific
                httpContext.Response.Headers.Append("vary", new[] { "accept", "accept-language", "x-dv-sig-1"});
                _logger.LogDebug($"{httpContext.Request.Method} ->  {httpContext.Request.Path}" );
                await next.Invoke();
            });
            
            app.UseCookiePolicy();
            
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("en"),
                    new CultureInfo("de")
                },
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en")
                }
            });


            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute().RequireAuthorization();
                endpoints.MapControllers(); // Map attribute-routed API controllers
                endpoints.MapRazorPages();
            });

            if (!string.IsNullOrWhiteSpace(Configuration["ASSET_BASE_PATH"]))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(Configuration["RELATIVE_ASSET_DIR"])) // If no relative Asset Dir configured, use default.
            {
                app.UseStaticFiles();
            }
            else
            {
                // For running from within an IDE, configure the Directory
                var currentDirectory = Directory.GetCurrentDirectory();
                var wwwroot = Path.Combine(currentDirectory, Configuration["RELATIVE_ASSET_DIR"]);

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(wwwroot), 
                    RequestPath = ""
                });
                
                if (env.IsDevelopment())
                {
                    app.UseDirectoryBrowser(new DirectoryBrowserOptions
                    {
                        FileProvider = new PhysicalFileProvider(wwwroot), 
                        RequestPath = ""
                    });
                }
            }
        }
    }

    public interface ICustomServiceProviderFactory
    {
        IServiceProvider CreateServiceProvider(IServiceCollection services);
    }
}


