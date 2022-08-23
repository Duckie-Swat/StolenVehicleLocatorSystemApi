using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public List<UserToken> UserTokens { get; set; }
    }
}
