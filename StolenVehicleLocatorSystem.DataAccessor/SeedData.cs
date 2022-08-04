using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StolenVehicleLocatorSystem.DataAccessor.Models;

namespace StolenVehicleLocatorSystem.DataAccessor
{
    public class SeedData
    {
        public static async Task EnsureSeedData(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            await EnsureRoles(scope);
        }

        private static async Task EnsureRoles(IServiceScope scope)
        {
            RoleManager<Role> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var admin = roleMgr.FindByNameAsync("Admin").Result;
            var customer = roleMgr.FindByNameAsync("Customer").Result;
            if (admin == null)
            {
                admin = new Role
                {
                    Name = "Admin",
                    NormalizedName = "admin"
                };
                var result = await roleMgr.CreateAsync(admin);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            if (customer == null)
            {
                customer = new Role
                {
                    Name = "Customer",
                    NormalizedName = "customer"
                };
                var result = await roleMgr.CreateAsync(customer);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
    }
}
