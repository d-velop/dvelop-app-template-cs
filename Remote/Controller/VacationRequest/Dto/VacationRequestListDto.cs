﻿using System.Collections.Generic;
using Dvelop.Sdk.Base.Dto;

namespace Dvelop.Remote.Controller.VacationRequest.Dto
{
    public class VacationRequestListDto: HalJsonDto
    {
        public List<VacationRequestDto> Values = new List<VacationRequestDto>();
    }
}