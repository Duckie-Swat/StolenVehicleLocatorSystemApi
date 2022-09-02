using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid CreatedBy { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public Guid DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual IEnumerable<UserToken> UserTokens { get; set; }
        public virtual IEnumerable<Notification> Notifications { get; set; }
        public virtual IEnumerable<Camera> Cameras { get; set; }
    }
}
