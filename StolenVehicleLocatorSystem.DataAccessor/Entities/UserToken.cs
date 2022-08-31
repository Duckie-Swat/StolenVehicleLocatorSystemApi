using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class UserToken : BaseEntity
    {
      
        public Guid UserId { get; set; }
        public string Platform { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
