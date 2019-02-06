using Dvelop.Remote.Controller.StatusCode.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Controller.StatusCode
{
    [ApiController]
    [AllowAnonymous]
    [Route("error")]
    public class StatusCodeController: Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StatusCodeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("status/{statusCode}", Name = nameof(StatusCodeController)+"."+nameof(GetErrorPage))]
        public ActionResult GetErrorPage(int statusCode)
        {
            var model = new StatusCodeModel
            {
                StatusCode = statusCode
            };

            return View("StatusCode", model);
        }
    }
}
