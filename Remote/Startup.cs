using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using dvelop.IdentityProvider.Client;
using dvelop.IdentityProvider.Client.AuthenticationHandler;
using dvelop.IdentityProvider.Client.Middleware;
using dvelop.TenantMiddleware;
using Dvelop.Domain.Repositories;
using Dvelop.Remote.Formatter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Dvelop.Remote
{
    public class Startup
    {
        private readonly ICustomServiceProviderFactory _factory;

        public Startup(IConfiguration configuration, ICustomServiceProviderFactory factory)
        {
            _factory = factory;
            Configuration = configuration;

            Configuration["DEFAULT_SYSTEM_BASE_URI"] = Configuration["SYSTEMBASEURI"]??"http://localhost";
            Configuration["SIGNATURE_SECRET"] = Configuration["SIGNATURE_SECRET"]??"";

            Configuration["APP_NAME"] = Configuration["APP_NAME"]??"vacationprocess-cs";
            Configuration["BASE"] = $"/{Configuration["APP_NAME"]}";
            Configuration["ASSETS"] = Configuration["ASSET_BASE_PATH"]??$"{Configuration["DEFAULT_SYSTEM_BASE_URI"]}{Configuration["BASE"]}";
            
            Console.WriteLine($"SYSTEMBASEURI: {Configuration["SYSTEMBASEURI"]}");
            Console.WriteLine($"DEFAULT_SYSTEM_BASE_URI: {Configuration["DEFAULT_SYSTEM_BASE_URI"]}");
            Console.WriteLine($"SIGNATURE_SECRET set: {!string.IsNullOrWhiteSpace(Configuration["SIGNATURE_SECRET"])}");
            Console.WriteLine($"APP_NAME: {Configuration["APP_NAME"]}");
            Console.WriteLine($"ASSETS: {Configuration["ASSETS"]}");
            Console.WriteLine($"BASE: {Configuration["BASE"]}");


        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Allow Classes to access the HttpContext
            services.AddHttpContextAccessor();
            
            // Enable d.ecs IdentityProvider
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "IdentityProvider";
                options.DefaultChallengeScheme = "IdentityProvider";
                options.DefaultForbidScheme = "IdentityProvider";

            }).AddIdentityProviderAuthentication("IdentityProvider", "d.velop Identity Provider", options =>
            {


            });
            
            // Create and configure Mvc
            services.AddMvc(options =>
                    {
                        // Enable Content Negotiation
                        options.RespectBrowserAcceptHeader = true;

                        // Remove Default Json Formatter
                        options.OutputFormatters.RemoveType<JsonOutputFormatter>();

                        // Insert HalJson Media-Formatter to allow serialization/deserialization of HypermediaApplicationLanguage
                        options.OutputFormatters.Insert(0, new HalJsonOutputFormatter());
                        
                        // Only Allow Authenticated User to access this application (Use [AllowAnonymous] to allow anonymous access)
                        
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));

    
                    }
                )
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1); // Should be set to 2.1 compatibility
            services.AddDirectoryBrowser();
            return _factory.CreateServiceProvider(services);
        }

        // This method gets called by the ASP .NET core runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           
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
                TriggerAuthentication = true,
                
                TenantInformationCallback = () =>
                {
                    var tenantRepository = app.ApplicationServices.GetService<ITenantRepository>();
                    return new TenantInformation
                    {
                        TenantId = tenantRepository.TenantId,
                        SystembaseUri = tenantRepository.SystemBaseUri.ToString()
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

            app.UsePathBase(Configuration["BASE"]);
           

            app.Use(async (httpContext, next) =>
            {
                httpContext.Response.Headers.Append("vary", new[] { "accept", "accept-language", "x-dv-sig-1"});
                Console.WriteLine($"{httpContext.Request.Host.Host} ->  {httpContext.Request.Path}" );
                await next.Invoke();
            });
            
            app.UseHttpsRedirection();
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
            
            app.UseMvc();
            
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
