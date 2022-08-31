using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace StolenVehicleLocatorSystem.DataAccessor.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}

