using Dvelop.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Dvelop.Plugins.WebApi
{
    public class AssetLocator: IAssetLocator
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IConfiguration _configuration;

        public AssetLocator(ITenantRepository tenantRepository, IConfiguration configuration)
        {
            _tenantRepository = tenantRepository;
            _configuration = configuration;
        }

        public string GetAssetsPath()
        {
            return _configuration["ASSET_BASE_PATH"] ??
                   $"{_tenantRepository.SystemBaseUri.OriginalString.TrimEnd('/')}{_configuration["BASE"]}";
        }
        
    }

    public interface IAssetLocator
    {
        public string GetAssetsPath();
    }
}