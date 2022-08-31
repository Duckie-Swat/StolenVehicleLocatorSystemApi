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

        public IEnumerable<UserToken> UserTokens { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
    }
}
