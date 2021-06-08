namespace ScheduleMessages.Infrastructure.EventHandling
{
    public class SendNotificationCommand :
        SendNotification
    {
        public string EmailAddress { get; set; }
        public string Body { get; set; }
    }
}