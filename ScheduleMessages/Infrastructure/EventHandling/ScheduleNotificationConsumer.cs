using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ScheduleMessages.Infrastructure.EventHandling
{
    public class ScheduleNotificationConsumer :
        IConsumer<ScheduleNotification>
    {

        private readonly ILogger<ScheduleNotificationConsumer> _logger;

        public ScheduleNotificationConsumer(ILogger<ScheduleNotificationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            //Uri notificationService = new Uri("queue:notification-service");
             _logger.LogInformation("I am here !");
            await context.ScheduleSend<SendNotification>(
                context.Message.DeliveryTime,
                new 
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body =  context.Message.Body
                });
        }
        
        
        
        
        
    }
}
