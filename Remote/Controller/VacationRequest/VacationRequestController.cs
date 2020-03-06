using System;
using System.Globalization;
using System.Linq;
using Dvelop.Domain.Vacation;
using Dvelop.Remote.Constraints;
using Dvelop.Remote.Controller.VacationRequest.Dto;
using Dvelop.Remote.Controller.VacationRequest.ViewModel;
using Dvelop.Sdk.Base.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Controller.VacationRequest
{
    /// <summary>
    /// Example for a controller using business logic and Views
    /// </summary>
    [Route("")]
    
    public class VacationRequestController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IVacationService _service;
        public const string ValuesRelation = "vacationrequests";

        public VacationRequestController(IVacationService service)
        {
            _service = service;
        }
        
        [ProducesConstraint("application/json", "application/hal+json")]
        [HttpGet("vacationrequest", Name = nameof(VacationRequestController) + "." +nameof(GetVacationList))]
        public VacationRequestListDto GetVacationList()
        {
            var dto = new VacationRequestListDto();
            dto.Values.AddRange(

            _service.Vacations.Select(vacation => {
                var vacationRequestDto = new VacationRequestDto
                {
                    Id = vacation.Id.ToString(),
                    To =  vacation.To,
                    From = vacation.From,
                    Comment = vacation.Comment,
                    State = vacation.State.ToString().ToLowerInvariant(),
                    Type = vacation.Type.ToString().ToLowerInvariant()
                };
                vacationRequestDto._links.Add("self", new RelationDataDto(Url.RouteUrl( nameof(VacationRequestController) + "." + nameof(GetVacationView),new {id = vacation.Id} )));
                return vacationRequestDto;
            }));
            
            dto._links.Add("self", new RelationDataDto(Url.RouteUrl( nameof(VacationRequestController) + "." + nameof(GetVacationList))));
            return dto;
        }
        
        [ProducesConstraint("text/html")]
        [HttpGet("vacationrequest", Name = nameof(VacationRequestController) + "." +nameof(GetVacationListView))]
        public ActionResult GetVacationListView()
        {
            ViewData["Title"] = "Vacation overview";
            ViewData["Js"] = "vacationrequestlist.js";
            ViewData["Css"] = "vacationrequestlist.css";
            
            var vacationRequestListView = new VacationRequestListView();
            vacationRequestListView.Values.AddRange( _service.Vacations.Select( vacation => new VacationRequestView
            {
                Id = vacation.Id.ToString(),
                Comment = vacation.Comment,
                From = vacation.From.ToShortDateString(),
                To = vacation.To.ToShortDateString(),
                State = vacation.State.ToString().ToLowerInvariant(),
                Type = vacation.Type.ToString().ToLowerInvariant()


            }));

            return View("VacationRequestList",vacationRequestListView);
        }


        
        [ProducesConstraint("application/json", "application/hal+json")]
        [HttpGet("vacationrequest/{id}", Name = nameof(VacationRequestController) + "." + nameof(GetVacation))]
        public VacationRequestDto GetVacation(string id)
        {
            var vacationModel = _service.Vacations.FirstOrDefault(x => x.Id.ToString() == id);
            if (vacationModel == null)
            {
                throw new ArgumentException();
            }
            var dto =  new VacationRequestDto
            {

                Id = vacationModel.Id.ToString(),
                To =  vacationModel.To,
                From = vacationModel.From,
                Comment = vacationModel.Comment,
                State = vacationModel.State.ToString().ToLowerInvariant(),
                Type = vacationModel.Type.ToString().ToLowerInvariant()
            };
            dto._links.Add("self", new RelationDataDto(Url.RouteUrl( nameof(VacationRequestController) + "." + nameof(GetVacation),new {id = dto.Id} )));
            return dto;
        }

        [ProducesConstraint("text/html")]
        [HttpGet("vacationrequest/{id}", Name = nameof(VacationRequestController) + "." + nameof(GetVacationView))]
        public ActionResult GetVacationView(string id)
        {
            ViewData["Title"] = "Vacationdetail";
            ViewData["Js"] = "vacationrequest.js";
            ViewData["Css"] = "vacationrequest.css";
            ViewData["DataMode"] = "edit";

            var vacationModel = _service.Vacations.FirstOrDefault(x => x.Id.ToString() == id);
            if (vacationModel == null)
            {
                throw new ArgumentException();
            }
            var vacationRequestView =  new VacationRequestView
            {

                Id = vacationModel.Id.ToString(),
                To =  vacationModel.To.ToString("yyyy-MM-dd",DateTimeFormatInfo.InvariantInfo),
                From = vacationModel.From.ToString("yyyy-MM-dd",DateTimeFormatInfo.InvariantInfo),
                Comment = vacationModel.Comment,
                State = vacationModel.State.ToString().ToLowerInvariant(),
                Type = vacationModel.Type.ToString().ToLowerInvariant()
            };
           
            ViewData["Title"] = $"Vacation {vacationModel.From.ToShortDateString()} - {vacationModel.To.ToShortDateString()}";
            return View("VacationRequest", vacationRequestView);
        }

        [ProducesConstraint("text/html")]
        [HttpGet("vacationrequestform", Name = nameof(VacationRequestController) + "." + nameof(GetVacationFormView))]
        public ActionResult GetVacationFormView(string id)
        {
            ViewData["Title"] = $"Create Vacation";
            ViewData["Js"] = "vacationrequest.js";
            ViewData["Css"] = "vacationrequest.css";
            ViewData["DataMode"] = "new";

            var vacationRequestView = new VacationRequestView();
            
            return View("VacationRequest", vacationRequestView);
        }

        [HttpPost("vacationrequest", Name = nameof(VacationRequestController) + "." +nameof(Post)) ]
        public ActionResult Post([FromBody] VacationRequestDto value)
        {
            var vacationModel = new VacationModel
            {
                Comment = value.Comment,
                From = value.From,
                To = value.To,
                
            };

            if (!string.IsNullOrWhiteSpace(value.Type))
            {
                vacationModel.Type = Enum.Parse<VacationType>(value.Type, true);
            }

            if (!string.IsNullOrWhiteSpace(value.State))
            {
                vacationModel.State = Enum.Parse<VacationState>(value.State, true);
            }

            var vacationRequest  = _service.RequestVacation(vacationModel);
            
            return Created(Url.RouteUrl(nameof(VacationRequestController) + "." + nameof(GetVacation),
                new {id = vacationRequest.ToString()}), _service.Vacations.FirstOrDefault(id=>id.Id == vacationRequest ));
        }

        
        [HttpPut("vacationrequest/{id}", Name = nameof(VacationRequestController) + "." + nameof(Put))]
        public ActionResult Put(string id, [FromBody] VacationRequestDto value)
        {
            var vacationModel = new VacationModel
            {
                Comment = value.Comment,
                From = value.From,
                To = value.To,
                Id = Guid.Parse(id)
            };

            if (!string.IsNullOrWhiteSpace(value.Type))
            {
                vacationModel.Type = Enum.Parse<VacationType>(value.Type, true);
            }

            if (!string.IsNullOrWhiteSpace(value.State))
            {
                vacationModel.State = Enum.Parse<VacationState>(value.State, true);
            }

            var success = _service.UpdateVacation(vacationModel);
            Console.WriteLine($" update success? { success }");
            return Ok();
        }

        [HttpPatch("vacationrequest/{id}", Name = nameof(VacationRequestController) + "." + nameof(Patch))]
        public void Patch(string id, [FromBody] VacationRequestStateDto value)
        {
            switch (value.State)
            {
                case "accepted":
                {
                    _service.AcceptVacation(Guid.Parse(id));
                    break;
                }
                case "rejected":
                {
                    _service.RejectVacation(Guid.Parse(id));
                    break;
                }
                case "cancelled":
                {
                    _service.CancelVacation(Guid.Parse(id));
                    break;
                }
                default:
                    throw new ArgumentException();
            }
        }
    }
}