using Dvelop.Domain.VersionService;
using Dvelop.Remote.Constraints;
using Dvelop.Remote.Controller.BusinessValue;
using Dvelop.Remote.Controller.ConfigFeature;
using Dvelop.Remote.Controller.HomeFeature;
using Dvelop.Remote.Controller.Root.Dto;
using Dvelop.Remote.Controller.Root.ViewModel;
using Dvelop.Remote.Controller.VacationRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Controller.Root
{
    [AllowAnonymous]    // Allows anonymous requests (for getting version information and /home-App features)
    [Route("/")]        // Binds to configured root route
    public class RootController: Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IVersionService _versionService;

        // Inject IVersionService
        public RootController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        /// <summary>
        /// Uses implementation of IVersionService to determine the Product Version.
        /// Returns a html-representation
        /// </summary>
        /// <returns>Version of this product</returns>
        [ProducesConstraint("application/json","application/hal+json")]
        [HttpGet(Name = nameof(RootController)+"."+nameof(GetJson))] 
        public RootDto GetJson()
        {
            var version = _versionService.Version;
            var versionDto = new RootDto
            {
                Major = version.Major,
                Minor = version.Minor,
                Patch = version.Patch,
                Qualifier = version.Qualifier
            };

            // Adding Features (HAL-LinkRelations)
            versionDto._links.Add(VacationRequestController.ValuesRelation,new RelationDataDto(Url.RouteUrl($"{nameof(VacationRequestController)}.{nameof(VacationRequestController.GetVacationList)}")));
            versionDto._links.Add(BusinessValueController.ValuesRelation,new RelationDataDto(Url.RouteUrl($"{nameof(BusinessValueController)}.{nameof(BusinessValueController.GetJsonList)}")));
            versionDto._links.Add(ConfigFeaturesController.ConfigFeatures, new RelationDataDto(Url.RouteUrl(nameof(ConfigFeaturesController)+"."+nameof(ConfigFeaturesController.GetConfigFeatures), null)));
            versionDto._links.Add(HomeFeatureController.FeaturesDescription, new RelationDataDto(Url.RouteUrl(nameof(HomeFeatureController)+"."+nameof(HomeFeatureController.GetFeaturesDescriptions))));
            return versionDto;
        }

        /// <summary>
        /// Uses implementation of IVersionService to determine the Product Version.
        /// Returns a html-representation
        /// </summary>
        /// <returns>Version of this product</returns>
        [ProducesConstraint("text/html")]
        [HttpGet(Name = nameof(RootController)+"."+nameof(GetHtml))] 
        public ActionResult GetHtml()
        {
            var version = _versionService.Version;
            var model = new RootView
            {
               Version = version.ToString()
            };
            ViewData["Title"] = "d.velop decs architecture app";
            return View("Root", model);
        }
    }
}
