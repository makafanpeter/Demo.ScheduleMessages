using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
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

        private readonly IMessageScheduler _publishEndpoint;

        public NotificationController(ILogger<NotificationController> logger, IMessageScheduler messageScheduler)
        {
            _logger = logger;
            _publishEndpoint = messageScheduler;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Notification scheduleNotification)
        {
            
            await  _publishEndpoint.SchedulePublish<ScheduleNotification>( 
                DateTime.UtcNow + TimeSpan.FromSeconds(180),
                new
            {
                 DeliveryTime = DateTime.Now.AddMinutes(1.5),
                 scheduleNotification.Body,
                 scheduleNotification.EmailAddress
            });

            return Ok();
        }
        
    }

    public class Notification
    {
        [Required]
        public string Body { get; set; }
        [Required, EmailAddress]
        public string EmailAddress { get; set; }
    }
}
