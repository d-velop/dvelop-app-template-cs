using System.Collections.Generic;

namespace Dvelop.Remote.Controller.BusinessValue.Dto
{
    public class BusinessValueListDto: HalJsonDto
    {
        public List<BusinessValueDto> Values = new List<BusinessValueDto>();
        public int TotalValue { get; set; }
    }
}