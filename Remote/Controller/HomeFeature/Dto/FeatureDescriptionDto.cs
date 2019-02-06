using System.Collections.Generic;

namespace Dvelop.Remote.Controller.HomeFeature.Dto
{
    public class FeatureDescriptionDto
    {
        public FeatureDescriptionDto()
        {
            Features = new List<FeatureDto>();
        }
        public List<FeatureDto> Features { get; set; }
    }
}