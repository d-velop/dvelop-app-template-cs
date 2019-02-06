using System.Collections.Generic;

namespace Dvelop.Remote.Controller.ConfigFeature.Dto
{
    public class CustomHeadlineDto
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public List<MenuItemDto> MenuItems { get; set; }

        public CustomHeadlineDto()
        {
            MenuItems = new List<MenuItemDto>();
        }
    }
}