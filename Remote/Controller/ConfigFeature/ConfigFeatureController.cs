using System.Collections.Generic;
using dvelop.IdentityProvider.Client;
using Dvelop.Domain.Repositories;
using Dvelop.Remote.Controller.ConfigFeature.Dto;
using Dvelop.Remote.Controller.VacationRequest;
using Microsoft.AspNetCore.Mvc;


namespace Dvelop.Remote.Controller.ConfigFeature
{
    /// <summary>
    /// Controller for /config-App Features
    /// </summary>
    [Route(ConfigFeatures)]
    public class ConfigFeaturesController : ControllerBase
    {
        public const string ConfigFeatures = "configfeatures";

        private readonly IUserRepository _userRepo;

        public ConfigFeaturesController (IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        /// <summary>
        /// Returns Features for /config-App
        /// in this example only administrative Users have Configuration-Features
        /// </summary>
        /// <returns>Configuration-Features</returns>
        [Route("", Name = nameof(ConfigFeaturesController)+"."+nameof(GetConfigFeatures))]
        [HttpGet]
        public ConfigFeatureDto GetConfigFeatures()
        {
            var configFeatureDto = new ConfigFeatureDto { AppName = "d.velop hackathon" };
            var customHeadlineDto = new CustomHeadlineDto
            {
                Caption = "d.velop hackathon",
                Description = $"Hello {_userRepo.CurrentUser.DvDisplayName}, this is a running example of d.ecs architecture cloud apps"
            };

            // Use these BuildIn Roles:
            // BUILT_IN_ADMIN_GROUP -> Group Configured as IDP-Admins
            // TENANT_ADMIN_GROUP   -> Group of administrative Cloud Users
            if (User.IsInRole(IdpConst.BUILT_IN_ADMIN_GROUP) || User.IsInRole(IdpConst.TENANT_ADMIN_GROUP))
            {
                customHeadlineDto.MenuItems.Add(new MenuItemDto
                {
                    Caption = "Pending vacations",
                    Description = "Accept or reject vacations",
                    Href = Url.RouteUrl(nameof(VacationRequestController) + "." + nameof(VacationRequestController.GetVacationListView),
                        null),

                    Keywords = new List<string>
                    {
                        "c#",
                        "hackathon",
                        "code",
                        "example",
                        "vacation"
                    }
                });
            }

            customHeadlineDto.MenuItems.Add(new MenuItemDto
            {
                Caption = "Request vacation",
                Description = "Your vacation",
                Href = Url.RouteUrl(nameof(VacationRequestController) + "." + nameof(VacationRequestController.GetVacationFormView),
                    null),

                Keywords = new List<string>
                {
                    "c#",
                    "hackathon",
                    "code",
                    "example",
                    "vacation"
                }
            });

            configFeatureDto.CustomHeadlines.Add(customHeadlineDto);
            return configFeatureDto;
        }
    }
}
