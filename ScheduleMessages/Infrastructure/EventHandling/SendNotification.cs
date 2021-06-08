namespace ScheduleMessages.Infrastructure.EventHandling
{
    public interface SendNotification
    {
        string EmailAddress { get; }
        string Body { get; }
    }
}