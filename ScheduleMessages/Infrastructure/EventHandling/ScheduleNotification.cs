using System;

namespace ScheduleMessages.Infrastructure.EventHandling
{
    public interface ScheduleNotification
    {
        DateTime DeliveryTime { get; }
        string EmailAddress { get; }
        string Body { get; }
    }
}