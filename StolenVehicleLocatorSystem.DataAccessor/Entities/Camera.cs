using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class Camera : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = "Unknown";

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual IEnumerable<CameraDetectedResult> CameraDetectedResults { get; set; }
    }
}
