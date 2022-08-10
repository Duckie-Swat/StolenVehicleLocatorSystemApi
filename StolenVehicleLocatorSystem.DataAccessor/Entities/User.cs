using Microsoft.AspNetCore.Identity;
using System;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
