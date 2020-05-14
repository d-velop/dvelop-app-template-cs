using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dvelop.Domain.Repositories;
using Dvelop.Sdk.SigningAlgorithms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        private readonly HttpClient _client;

        public AppSessionController(IConfiguration configuration, ITenantRepository tenantRepository, IUserRepository user, IHttpClientFactory client, ILoggerFactory factory)
        {
            _configuration = configuration;
            _tenantRepository = tenantRepository;
            _user = user;
            _client = client.CreateClient("test");
            _log = factory.CreateLogger<AppSessionController>();
        }
        
        [AllowAnonymous]
        [HttpGet("appsession", Name = nameof(AppSessionController) + "." + nameof(CreateAppSession))]
        public async Task<IActionResult> CreateAppSession()
        {
            var requestId = Guid.NewGuid().ToString("N");

            var route = Url.RouteUrl(nameof(AppSessionController) + "." + nameof(Callback), new {requestId});
            
            var appSessionRequestDto = new AppSessionRequestDto
            {
                RequestId = HmacSha256Algorithm.HmacSha256( Convert.FromBase64String(_configuration["SIGNATURE_SECRET"]), requestId),
                AppName = _configuration["APP_NAME"],
                Callback = new Uri( route , UriKind.Relative)
            };
            
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_tenantRepository.SystemBaseUri, "/identityprovider/appsession"))
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(appSessionRequestDto,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new LowercaseNamingStrategy()
                            }
                        }), Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Origin", _tenantRepository.SystemBaseUri.ToString().TrimEnd('/'));

            var response = await _client.SendAsync(request);
              
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            return StatusCode((int) response.StatusCode, response.ReasonPhrase);
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
            var mySign = HmacSha256Algorithm.Sha256(
                $"{_configuration["APP_NAME"]}{callback.AuthSessionId}{callback.Expire}{hashedRequestId}");
            
            // Hex(Sha256(<appname><authSessionId><expire><requestid>
            
            _log.LogInformation($"sign valid? {mySign==callback.Sign}");
            if (mySign == callback.Sign)
            {
                return Ok(new StringContent(callback.AuthSessionId));
            }
            else
            {
                return BadRequest("Sign mismatch");
            }
        }
    }

    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name.ToLowerInvariant();
        }
    }

    public class AppSessionRequestDto
    {
        public string AppName { get; set; }
        public Uri Callback { get; set; }
        public string RequestId { get; set; }
    }
    
    public class AppSessionCallbackDto
    {
        public string AuthSessionId { get; set; }
        public string Expire { get; set; }
        public string Sign { get; set; }
    }
}