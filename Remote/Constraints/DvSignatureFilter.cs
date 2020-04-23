using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dvelop.Remote.Constraints
{
    public class DvSignatureFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<DvSignatureFilter> _log;
        public DvSignatureFilter(IConfiguration configuration, ILoggerFactory factory)
        {
            _configuration = configuration;
            _log = factory.CreateLogger<DvSignatureFilter>();
        }
        

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _log.LogInformation("Starting Cloud Event Signature verification.");

            if (!"DV1-HMAC-SHA256".Equals(context.HttpContext.Request.Headers["x-dv-signature-algorithm"], StringComparison.InvariantCultureIgnoreCase))
            {
                context.Result = new BadRequestResult();
                return;
            }
            
            var signatureHash = await Dv1HmacSha256Algorithm.CalculateSignature(context.HttpContext.Request, _configuration["SIGNATURE_SECRET"]);
            if (signatureHash == null)
            {
                _log.LogInformation("Cloud Event Signature verification failed.");
                context.Result = new ForbidResult();
                return;
            }

            string authorization = context.HttpContext.Request.Headers["Authorization"];
            if (authorization.Split(' ').Length != 2 && authorization.Split(' ')[0] != "Bearer")
            {
                _log.LogInformation("Cloud Event Signature verification failed.");
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