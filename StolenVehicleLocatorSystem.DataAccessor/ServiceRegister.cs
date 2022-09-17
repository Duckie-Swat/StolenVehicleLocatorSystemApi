using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StolenVehicleLocatorSystem.DataAccessor.Data;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.DataAccessor.Repositories;

namespace StolenVehicleLocatorSystem.DataAccessor;

public static class ServiceRegister
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // For Entity Framework  
        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
                ,
                ServiceLifetime.Transient
            );
        // For Identity  
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;
        });

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }
}
