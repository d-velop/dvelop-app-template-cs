using System;

namespace Dvelop.Remote.Controller.VacationRequest.Dto
{
    public class VacationRequestDto: HalJsonDto
    {
        public string State { get; set; }
        public string Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
}