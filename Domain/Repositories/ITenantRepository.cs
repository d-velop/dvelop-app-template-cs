using System;

namespace Dvelop.Domain.Repositories
{
    public interface ITenantRepository
    {
        string TenantId { get; set;}
        Uri SystemBaseUri { get; set;}
    }
}
