using System;
using Dvelop.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Dvelop.Plugins.WebApi
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IHttpContextAccessor _context;

        private static readonly string TenantIdKey = $"{nameof(ITenantRepository)}.{nameof(TenantId)}";
        private static readonly string SystemBaseUriKey = $"{nameof(ITenantRepository)}.{nameof(SystemBaseUri)}";

        public TenantRepository(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string TenantId
        {
            get => _context.HttpContext.Items[TenantIdKey] as string;
            set => _context.HttpContext.Items[TenantIdKey] = value;
        }

        public Uri SystemBaseUri { 
            get => _context.HttpContext.Items[SystemBaseUriKey] as Uri;
            set => _context.HttpContext.Items[SystemBaseUriKey] = value;
        }
    }
}
