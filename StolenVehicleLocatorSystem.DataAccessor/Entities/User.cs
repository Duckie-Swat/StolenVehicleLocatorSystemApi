using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class User : IdentityUser<Guid>
    {
        public List<UserToken> UserTokens { get; set; }
    }
}
