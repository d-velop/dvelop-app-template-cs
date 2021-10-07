
using Dvelop.Plugins.WebApi;
using Dvelop.Remote.Controller.VacationRequest;
using Dvelop.Sdk.Home.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dvelop.Remote.Controller.HomeFeature
{
    /// <summary>
    /// Controller for /home-App Features
    /// </summary>
    [Route("features")]
    public class HomeFeatureController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAssetLocator _assetLocator;
        public const string FeaturesDescription = "featuresdescription";

        public HomeFeatureController(IConfiguration configuration, IAssetLocator assetLocator)
        {
            _configuration = configuration;
            _assetLocator = assetLocator;
        }
        
        /// <summary>
        /// Creates a List of Features, to be shown at the /home-App
        /// </summary>
        /// <returns>Description of Features</returns>
        [Route("description", Name = nameof(HomeFeatureController)+"."+nameof(GetFeaturesDescriptions))]
        [HttpGet]
        public FeatureDescriptionDto GetFeaturesDescriptions()
        {
            return BuildFeatureDescriptionDto();
        }

        private FeatureDescriptionDto BuildFeatureDescriptionDto()
        {
            var featureListDto = new FeatureDescriptionDto();
            
            var feature = BuildFeatureDto();
            featureListDto.Features.Add(feature);
            
            return featureListDto;
        }

        private FeatureDto BuildFeatureDto()
        {
            var feature = new FeatureDto
            {
                Title = "Vacation Process",
                SubTitle = "Example of d.velop cloud platform integration",
                Summary = "Extend the d.velop cloud platform",
                Url = Url.RouteUrl(nameof(VacationRequestController) + "." +nameof(VacationRequestController.GetVacationListView)),
                Color = "pumpkin",
                IconUri = $"{_assetLocator.GetAssetsPath()}/icon_white.png",
                Description = "Learn to create a d.ecs architecture application for extending the d.velop cloud platform."
            };
            return feature;
        }
    }
}