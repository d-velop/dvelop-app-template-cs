using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dvelop.Domain.Repositories;
using Dvelop.Sdk.IdentityProvider.Client;
using Dvelop.Sdk.IdentityProvider.Dto;
using Dvelop.Sdk.SigningAlgorithms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dvelop.Remote.Controller.AppSession
{
   
    [Route("")]
    public class AppSessionController  : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<AppSessionController> _log;
        private readonly IdentityProviderClient _idpClient;

        
        public AppSessionController(IConfiguration configuration, ITenantRepository tenantRepository, ILoggerFactory factory)
        {
            _configuration = configuration;
            _tenantRepository = tenantRepository;
            _log = factory.CreateLogger<AppSessionController>();
            
            _idpClient = new IdentityProviderClient( new IdentityProviderClientOptions
            {
                SystemBaseUri = new Uri(_configuration["DEFAULT_SYSTEM_BASE_URI"]),
                TenantInformationCallback = () =>
                {
                    _log.LogInformation($"Tenant Identified {_tenantRepository.TenantId} {_tenantRepository.SystemBaseUri}");
                    return new TenantInformation
                    {
                        TenantId = tenantRepository.TenantId,
                        SystemBaseUri = tenantRepository.SystemBaseUri.ToString()
                    };

                },
                AllowAppSessions = true,
                LogCallBack = (level, s) =>
                {
                    switch (level)
                    {
                        case IdentityProviderClientLogLevel.Debug:
                            _log.LogDebug(s);
                            break;
                        case IdentityProviderClientLogLevel.Info:
                            _log.LogInformation(s);
                            break;
                        case IdentityProviderClientLogLevel.Warning:
                            _log.LogWarning(s);
                            break;
                        case IdentityProviderClientLogLevel.Error:
                            _log.LogError(s);
                            break;
                        default:
                            _log.LogInformation(s);
                            break;
                    }
                }
            });
            
        }
        
        [HttpGet("appsession", Name = nameof(AppSessionController) + "." + nameof(CreateAppSession))]
        public async Task<IActionResult> CreateAppSession()
        {
            _log.LogInformation($"Requesting AppSession");
            
            //Step 1: Lookup a persistent Database, which should be tenant and expire aware.
            if (!string.IsNullOrWhiteSpace(_configuration[_tenantRepository.TenantId]))
            {
                await DoSomethingAsApp(_configuration[_tenantRepository.TenantId]);

                var contains =string.IsNullOrEmpty(_configuration[_tenantRepository.TenantId]);
                
                // This InMemory Store don't work in AWS lambda
                return Ok(contains?"I already know the AppSession-Token":"Take a look into your logs, the AppSession is logged there");
            }
            var requestId = Guid.NewGuid().ToString("N");

            var route = Url.RouteUrl(nameof(AppSessionController) + "." + nameof(Callback), new {requestId});
            
            var appSessionRequestDto = new AppSessionRequestDto
            {
                RequestId = HmacSha256Algorithm.HmacSha256( Convert.FromBase64String(_configuration["SIGNATURE_SECRET"]), requestId),
                AppName = _configuration["APP_NAME"],
                Callback = new Uri( route , UriKind.Relative)
            };
            _log.LogInformation("RequestId: " + appSessionRequestDto.RequestId);
            _log.LogInformation("AppName: " + appSessionRequestDto.AppName);
            _log.LogInformation("CallBack: " + appSessionRequestDto.Callback);
            
            //Step 2: request a new AppSession from IDP
            if (await _idpClient.RequestAppSession( appSessionRequestDto ))
            {
                //Step 5: The IDP requests succeeded not the Appsession is stored in out database
                await DoSomethingAsApp(_configuration[_tenantRepository.TenantId]??"");
                return Ok($"Take a look into your logs, the AppSession is logged there: {_configuration[_tenantRepository.TenantId]}");
            }
            _log.LogWarning("There was an error while creating an AppSession");
            return BadRequest("The AppSession could not be created");
        }
        
        [AllowAnonymous]
        [HttpPost( "appsession/callback/{requestId}/", Name = nameof(AppSessionController) + "." + nameof(Callback))]
        public IActionResult Callback(string requestId, [FromBody] AppSessionCallbackDto callback)
        {
            //Step 3: This callback will be executed, while the request to the IDP is still pending
            _log.LogInformation( "Sign: " + callback.Sign );
            _log.LogInformation( "AppSessionId: " + callback.AuthSessionId );
            _log.LogInformation( "Expire: "+callback.Expire);
            _log.LogInformation( "RequestId: " + requestId );
            var hashedRequestId =  HmacSha256Algorithm.HmacSha256(Convert.FromBase64String(_configuration["SIGNATURE_SECRET"]), requestId);
            var mySign = HmacSha256Algorithm.Sha256($"{_configuration["APP_NAME"]}{callback.AuthSessionId}{callback.Expire}{hashedRequestId}");
            
            
            _log.LogInformation($"signature valid? {mySign==callback.Sign}");
            if (mySign != callback.Sign)
            {
                //Only accept valid requests
                _log.LogWarning("Signature mismatch");
                return BadRequest("Sign mismatch");
            }
            
            //Step 4: persist the created AppSession for later use
            _configuration[_tenantRepository.TenantId] = callback.AuthSessionId;
            return Ok(new StringContent("Got an AppSession", Encoding.UTF8, "application/json"));
        }

        private async Task<bool> DoSomethingAsApp(string appSessionToken)
        {
            _log.LogInformation($"working with an AppSession-Token: {appSessionToken}");
            //This will do something important as App
            var claim = await _idpClient.GetClaimsPrincipalAsync(appSessionToken);
            
            return true;
        }
    }
}