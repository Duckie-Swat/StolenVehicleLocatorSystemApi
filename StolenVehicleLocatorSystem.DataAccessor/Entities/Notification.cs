using StolenVehicleLocatorSystem.DataAccessor.Enums;
using System;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NotificationTypeEnum  Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUnRead { get; set; } = true;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
