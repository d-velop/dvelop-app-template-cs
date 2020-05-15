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
    [AllowAnonymous]
    [Route("")]
    public class AppSessionController  : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _user;

        private readonly ILogger<AppSessionController> _log;
        private readonly IdentityProviderClient _idpClient;

        private static readonly Dictionary<string, string> AppSessionStore = new Dictionary<string, string>();
        
        
        public AppSessionController(IConfiguration configuration, ITenantRepository tenantRepository, IUserRepository user, IHttpClientFactory client, ILoggerFactory factory)
        {
            _configuration = configuration;
            _tenantRepository = tenantRepository;

            _user = user;
            _idpClient = new IdentityProviderClient( client, () => new TenantInformation
            {
                TenantId = tenantRepository.TenantId,
                SystemBaseUri = tenantRepository.SystemBaseUri.ToString()
            } );
            _log = factory.CreateLogger<AppSessionController>();
        }
        
        [AllowAnonymous]
        [HttpGet("appsession", Name = nameof(AppSessionController) + "." + nameof(CreateAppSession))]
        public async Task<IActionResult> CreateAppSession()
        {
            _log.LogInformation($"Requesting AppSession");
            
            // Lookup a persistent Database, which should be tenant and expire aware.
            if (AppSessionStore.ContainsKey(_tenantRepository.TenantId) && !string.IsNullOrWhiteSpace(AppSessionStore[_tenantRepository.TenantId]))
            {
                return Ok(AppSessionStore[_tenantRepository.TenantId]);
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
            
            if (await _idpClient.RequestAppSession( appSessionRequestDto ))
            {
                _log.LogInformation("successfully created an AppSession for " + _tenantRepository.TenantId);
                _log.LogInformation($"number of AppSessions in Store: {AppSessionStore.Count}");
                foreach (var keyValuePair in AppSessionStore)
                {
                    _log.LogInformation( $"{keyValuePair.Key} -> {keyValuePair.Value} " );
                }
                var appSessionNotYetAvailable = (AppSessionStore.ContainsKey(_tenantRepository.TenantId) && !string.IsNullOrWhiteSpace(AppSessionStore[_tenantRepository.TenantId]) 
                    ? AppSessionStore[_tenantRepository.TenantId] : "AppSession not yet available");
                return Ok(appSessionNotYetAvailable);
            }
            _log.LogWarning("There was an error while creating an AppSession");
            return BadRequest("The AppSession could not be created");
        }
        
        [AllowAnonymous]
        [HttpPost( "appsession/callback/{requestId}/", Name = nameof(AppSessionController) + "." + nameof(Callback))]
        public IActionResult Callback(string requestId, [FromBody] AppSessionCallbackDto callback)
        {
            _log.LogInformation( "Sign: " + callback.Sign );
            _log.LogInformation( "AppSessionId:" +callback.AuthSessionId );
            _log.LogInformation( "Expire: "+callback.Expire);
            _log.LogInformation( "RequestId: " + requestId );
            var hashedRequestId =  HmacSha256Algorithm.HmacSha256(Convert.FromBase64String(_configuration["SIGNATURE_SECRET"]), requestId);
            var mySign = HmacSha256Algorithm.Sha256($"{_configuration["APP_NAME"]}{callback.AuthSessionId}{callback.Expire}{hashedRequestId}");
            
            _log.LogInformation($"signature valid? {mySign==callback.Sign}");
            if (mySign != callback.Sign)
            {
                _log.LogWarning("Signature mismatch");
                return BadRequest("Sign mismatch");
            }
            _log.LogInformation($"number of AppSessions in Store: {AppSessionStore.Count}");
            foreach (var keyValuePair in AppSessionStore)
            {
                _log.LogInformation( $"{keyValuePair.Key} -> {keyValuePair.Value} " );
            }
            AppSessionStore[_tenantRepository.TenantId] = callback.AuthSessionId;
            return Ok(new StringContent("Got an AppSession", Encoding.UTF8, "application/json"));
        }
    }
}