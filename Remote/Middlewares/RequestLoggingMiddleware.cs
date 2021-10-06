using System.Diagnostics;
using System.Threading.Tasks;
using Dvelop.Domain.Repositories;
using Dvelop.Sdk.Logging.Abstractions.Scope;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dvelop.Remote.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
 
        private readonly RequestDelegate _next;
        private readonly ITenantRepository _tenantRepository;

        public RequestLoggingMiddleware(RequestDelegate next, ITenantRepository tenantRepository, ILogger<RequestLoggingMiddleware> logger)
        {
            _logger = logger;
            _next = next;
            _tenantRepository = tenantRepository;
        }
 
        public async Task InvokeAsync(HttpContext context)
        {
            using (_logger.BeginScope(new TracingLogScope(Activity.Current?.TraceId.ToString(), Activity.Current?.SpanId.ToString())))
            using (_logger.BeginScope(new TenantLogScope(_tenantRepository?.TenantId)))
            {
                await _next(context);
            }
        }
    }

}