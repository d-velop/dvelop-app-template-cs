using System.Collections.Generic;

namespace Dvelop.Remote.Controller.ConfigFeature.Dto
{
    public class PredefinedHeadlineDto
    {
        public string Group { get; set; }
        public List<MenuItemDto> MenuItems { get; set; }

        public PredefinedHeadlineDto()
        {
            MenuItems = new List<MenuItemDto>();
        }
    }
}