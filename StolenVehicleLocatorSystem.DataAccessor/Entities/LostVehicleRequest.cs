using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class LostVehicleRequest : BaseEntity
    {
        public Guid UserId { get; set; }

        public string PlateNumber { get; set; }

        public string VehicleType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Location { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
