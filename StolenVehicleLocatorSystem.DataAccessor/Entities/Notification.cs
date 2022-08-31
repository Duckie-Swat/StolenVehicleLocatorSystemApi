using StolenVehicleLocatorSystem.DataAccessor.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class Notification : BaseEntity
    {
       
        public string Title { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }
        public NotificationTypeEnum  Type { get; set; }
            
        
        public bool IsUnRead { get; set; } = true;

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
