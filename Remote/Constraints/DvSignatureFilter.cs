using System;
using System.IO;
using System.Threading.Tasks;
using Dvelop.SDK.SigningAlgorithms.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dvelop.Remote.Constraints
{
    public class DvSignatureFilter : IAsyncAuthorizationFilter
    {
        private readonly string _secret;

        private readonly ILogger<DvSignatureFilter> _log;
        
        public DvSignatureFilter(IConfiguration configuration, ILoggerFactory factory)
        {
            _secret = configuration["SIGNATURE_SECRET"];
            _log = factory.CreateLogger<DvSignatureFilter>();
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var abc = context.RouteData;
            
            _log.LogInformation("Starting Cloud Event Signature verification.");

            
            
            var httpContextRequest = context.HttpContext.Request;
            
            
            
            httpContextRequest.EnableBuffering();
            httpContextRequest.Body.Seek(0, SeekOrigin.Begin);
            var m = new MemoryStream();
            await httpContextRequest.Body.CopyToAsync(m);
            httpContextRequest.Body.Seek(0, SeekOrigin.Begin);
            m.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(m);
            var text = await reader.ReadToEndAsync();
            
            _log.LogInformation(text);
            _log.LogInformation($"1 {httpContextRequest.Method} {httpContextRequest.PathBase}{httpContextRequest.Path}");
            _log.LogInformation("2 "+new Uri(httpContextRequest.GetDisplayUrl(), UriKind.RelativeOrAbsolute).AbsolutePath);
            _log.LogInformation("3 " +httpContextRequest.GetDisplayUrl());
                
            
            
            foreach (var (key, value) in httpContextRequest.Headers)
            {
                _log.LogInformation($"{key}:{value}");
            }
            
            if (!"DV1-HMAC-SHA256".Equals(httpContextRequest.Headers["x-dv-signature-algorithm"], StringComparison.InvariantCultureIgnoreCase))
            {
                _log.LogInformation("Cloud Event Signature missing.");
                context.Result = new BadRequestResult();
                return;
            }
            
            var signatureHash = await httpContextRequest.CalculateDv1HmacSha256Signature( _secret );
            if (signatureHash == null)
            {
                _log.LogInformation("Cloud Event Signature calculation failed.");
                context.Result = new ForbidResult();
                return;
            }

            string authorization = httpContextRequest.Headers["Authorization"];
            if (authorization.Split(' ').Length != 2 && authorization.Split(' ')[0] != "Bearer")
            {
                _log.LogInformation("Cloud Event Signature checksum missing.");
                context.Result = new ForbidResult();
                return;
            }

            authorization = authorization.Split(' ')[1];

            if (signatureHash != authorization)
            {
                _log.LogInformation("Cloud Event Signature verification failed.");
                context.Result = new ForbidResult();
            }
            _log.LogInformation("Cloud Event Signature verification ended.");
        }
    }
}