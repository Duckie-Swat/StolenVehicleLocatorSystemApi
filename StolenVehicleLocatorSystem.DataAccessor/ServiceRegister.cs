using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StolenVehicleLocatorSystem.DataAccessor.Context;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.DataAccessor.Models;
using StolenVehicleLocatorSystem.DataAccessor.Repository;

namespace StolenVehicleLocatorSystem.DataAccessor
{
    public static class ServiceRegister
    {
        public static void AddDataAccessorLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoContext, MongoContext>();
            services.AddSingleton<IUserRepository, UserRepository>();

            var mongoDbSettings = configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();
            services.AddIdentity<User, Role>()
            .AddMongoDbStores<User, Role, Guid>
            (
                mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName
            )
            .AddDefaultTokenProviders();

        }
    }
}
