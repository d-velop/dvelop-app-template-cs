using System.Collections.Generic;

namespace Dvelop.Remote.Controller.VacationRequest.Dto
{
    public class VacationRequestListDto: HalJsonDto
    {
        public List<VacationRequestDto> Values = new List<VacationRequestDto>();
    }
}