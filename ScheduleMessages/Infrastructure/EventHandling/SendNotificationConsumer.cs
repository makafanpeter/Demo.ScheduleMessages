using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ScheduleMessages.Infrastructure.EventHandling
{
    public class SendNotificationConsumer: IConsumer<SendNotification>
    {
        private readonly ILogger<SendNotificationConsumer> _logger;

        public SendNotificationConsumer(ILogger<SendNotificationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendNotification> context)
        {
            
            _logger.LogInformation("I am here");
            
        }
    }
}