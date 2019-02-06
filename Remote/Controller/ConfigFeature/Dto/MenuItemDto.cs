using System.Collections.Generic;

namespace Dvelop.Remote.Controller.ConfigFeature.Dto
{
    public class MenuItemDto
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public List<string> Keywords { get; set; }
        public ConfigurationStateDto ConfigurationState { get; set; }

        public MenuItemDto()
        {
            Keywords = new List<string>();
        }
    }
}