using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System;

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
            builder.Entity<LostVehicleRequest>().HasIndex(u => u.PlateNumber).IsUnique();
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<LostVehicleRequest> LostVehicleRequests { get; set; }
    }
}

