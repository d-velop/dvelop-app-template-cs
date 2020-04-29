using Dvelop.Remote.Constraints;
using Dvelop.SDK.CloudCenter.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dvelop.Remote.Controller.LifeCycleEvents
{
    [DvSignatureAuthentication]
    public class DvelopLifeCycleEventController: ControllerBase
    {
        private readonly ILogger<DvelopLifeCycleEventController> _log;

        public DvelopLifeCycleEventController(ILoggerFactory factory)
        {
            _log = factory.CreateLogger<DvelopLifeCycleEventController>();
        }
        
        [Route("dvelop-cloud-lifecycle-event")]
        public ActionResult Post([FromBody] CloudCenterEventDto eventDto)
        {
            _log.LogInformation( $"Got Event {eventDto?.Type} for {eventDto?.TenantId} -> {eventDto?.BaseUri}" );
            return new OkResult();
        }
    }
}