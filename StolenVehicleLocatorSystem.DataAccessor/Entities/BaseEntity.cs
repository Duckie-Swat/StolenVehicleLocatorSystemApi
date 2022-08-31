using System;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid CreatedBy { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public Guid DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
