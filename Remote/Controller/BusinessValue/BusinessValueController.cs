using System.Linq;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Remote.Constraints;
using Dvelop.Remote.Controller.BusinessValue.Dto;
using Dvelop.Remote.Controller.BusinessValue.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Controller.BusinessValue
{
    /// <summary>
    /// Example for a controller using business logic and Views
    ///
    /// Binds to 'business/value' -> /hackathon/business/value
    /// </summary>
    [Route("business/value")]
    public class BusinessValueController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IExampleBusinessLogicService _service;
        public const string ValuesRelation = "values";

        public BusinessValueController(IExampleBusinessLogicService service)
        {
            _service = service;
        }
        
        [ProducesConstraint("application/json", "application/hal+json")]
        [HttpGet("", Name = nameof(BusinessValueController) + "." +nameof(GetJsonList))]
        public BusinessValueListDto GetJsonList()
        {
            var dto = new BusinessValueListDto();
            dto.Values.AddRange(_service.ValuesList.Select(x =>
            {
                var bv =  new BusinessValueDto
                {
                    TotalCustomerValue = x.TotalCustomerValue,
                    Customer = x.Customer,
                    Id = x.Id
                };
                bv._links.Add("self", new RelationDataDto(Url.RouteUrl( nameof(BusinessValueController) + "." + nameof(GetBusinessValueHtml),new {id = x.Id} )));
                return bv;
            }));
            dto._links.Add("self", new RelationDataDto(Url.RouteUrl( nameof(BusinessValueController) + "." + nameof(GetJsonList))));
            dto.TotalValue = _service.TotalValue();
            return dto;
        }
        
        [ProducesConstraint("text/html")]
        [HttpGet("", Name = nameof(BusinessValueController) + "." +nameof(GetHtmlList))]
        public ActionResult GetHtmlList()
        {
            ViewData["Title"] = "List of Customer.";
            var businessValueListView = new BusinessValueListView
            {
                Caption = "Customer List",
                Total = _service.TotalValue()
            };

            businessValueListView.Values.AddRange(_service.ValuesList.Select(x =>
            {
                var bv =  new BusinessValueListViewEntry
                {
                    Value = x.TotalCustomerValue,
                    Caption = x.Customer,
                    DetailLink = Url.RouteUrl( nameof(BusinessValueController) + "." + nameof(GetBusinessValueHtml),new {id = x.Id} )
                };
                return bv;
            }));
            return View("BusinessValueList",businessValueListView);
        }

        
        [ProducesConstraint("application/json", "application/hal+json")]
        [HttpGet("{id}", Name = nameof(BusinessValueController) + "." + nameof(GetBusinessValueJson))]
        public BusinessValueDto GetBusinessValueJson(int id)
        {
            var v = _service.ValuesList[id];
            var dto =  new BusinessValueDto
            {
                Id = v.Id,
                TotalCustomerValue = v.TotalCustomerValue,
                Customer = v.Customer
            };
            dto._links.Add("self", new RelationDataDto(Url.RouteUrl(nameof(BusinessValueController) + "." + nameof(GetBusinessValueJson))));
            return dto;
        }

        [ProducesConstraint("text/html")]
        [HttpGet("{id}", Name = nameof(BusinessValueController) + "." + nameof(GetBusinessValueHtml))]
        public ActionResult GetBusinessValueHtml(int id)
        {
            
            var v = _service.ValuesList[id];
            var businessValueView =  new BusinessValueView
            {
                TotalCustomerValue = v.TotalCustomerValue,
                Customer = v.Customer
            };
            ViewData["Title"] = "Value of " + v.Customer;
            return View("BusinessValue", businessValueView);
        }

        [HttpPost("", Name = nameof(BusinessValueController) + "." + nameof(BusinessValueController)+nameof(Post)) ]
        public void Post([FromBody] BusinessValueDto value)
        {
            _service.ValuesList.Add(new Domain.ExampleBusinessLogic.BusinessValue
            {
                TotalCustomerValue =value.TotalCustomerValue,
                Customer = value.Customer,
                Id = _service.ValuesList.Count
            });
        }

        [HttpPut("{id}", Name = nameof(BusinessValueController) + "." + nameof(BusinessValueController)+nameof(Put))]
        public void Put(int id, [FromBody] BusinessValueDto value)
        {
            _service.ValuesList[id] = new Domain.ExampleBusinessLogic.BusinessValue
            {
                TotalCustomerValue = value.TotalCustomerValue,
                Customer = value.Customer,
                Id = value.TotalCustomerValue
            };
        }

        [HttpDelete("{id}", Name = nameof(BusinessValueController) + "." + nameof(BusinessValueController)+nameof(Delete))]
        public void Delete(int id)
        {
            _service.ValuesList.RemoveAt(id);
        }
    }
}