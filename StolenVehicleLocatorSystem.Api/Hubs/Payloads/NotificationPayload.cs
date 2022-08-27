using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;

namespace StolenVehicleLocatorSystem.Api.Hubs.Payloads
{
    public class NotificationPayload
    {
        public string? To { get; set; }
        public CreateNotificationDto Content { get; set; }
    }
}
