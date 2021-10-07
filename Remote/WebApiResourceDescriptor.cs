using System;
using Dvelop.Domain.VersionService;
using Dvelop.Sdk.Logging.Abstractions.Resource;
using Microsoft.Extensions.Configuration;

namespace Dvelop.Remote
{
    public class WebApiResourceDescriptor: IResourceDescriptor
    {
        private readonly IVersionService _versionService;
        private readonly IConfiguration _configuration;

        public WebApiResourceDescriptor(IVersionService versionService, IConfiguration configuration)
        {
            _versionService = versionService;
            _configuration = configuration;
        }
        public ResourceInfo GetResourceInfo()
        {
            return new ResourceInfo(new ServiceInfo(_configuration["APP_NAME"]??"", _versionService.Version.ToString(), Guid.NewGuid().ToString()), null, null);
        }
    }
}