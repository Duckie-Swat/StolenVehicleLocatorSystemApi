﻿using Microsoft.AspNetCore.Identity;
using System;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid CreatedBy { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public Guid DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}