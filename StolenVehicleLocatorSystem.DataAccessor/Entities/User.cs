using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<UserToken> UserTokens { get; set; }
    }
}
