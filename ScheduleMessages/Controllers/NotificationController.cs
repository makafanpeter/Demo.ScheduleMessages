using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MassTransit;
using ScheduleMessages.Infrastructure.EventHandling;

namespace ScheduleMessages.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {

        private readonly ILogger<NotificationController> _logger;

        private readonly IPublishEndpoint _publishEndpoint;

        public NotificationController(ILogger<NotificationController> logger, IPublishEndpoint messageScheduler)
        {
            _logger = logger;
            _publishEndpoint = messageScheduler;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SendNotificationCommand scheduleNotification)
        {
            
            await  _publishEndpoint.Publish<ScheduleNotification>( new
            {
                 DeliveryTime = DateTime.Now.AddMinutes(1.5),
                 scheduleNotification.Body,
                 scheduleNotification.EmailAddress
            });

            return Ok();
        }
        
    }
}
