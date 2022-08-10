using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Business.Services;
using StolenVehicleLocatorSystem.DataAccessor;
using System.Reflection;

namespace StolenVehicleLocatorSystem.Business
{
    public static class ServiceRegister
    {
        public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccessLayer(configuration);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
        }

    }
}
