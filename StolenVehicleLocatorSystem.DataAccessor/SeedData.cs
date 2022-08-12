using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StolenVehicleLocatorSystem.DataAccessor.Data;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System;
using System.Linq;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.DataAccessor
{
    public static class SeedData
    {
        public static void EnsureSeedData(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>();
            ctx.Database.Migrate();
            EnsureRoles(scope);
            EnsureUsers(scope);
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            UserManager<User> userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            User admin = userMgr.FindByNameAsync("admin").Result;
            User customer = userMgr.FindByNameAsync("customer").Result;
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "admin",
                    Email = "admin@duckieswat.com",
                    EmailConfirmed = true,
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true
                };
                var result = userMgr.CreateAsync(admin, "Str0ng!Passw0rd").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(admin, "Admin").Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result =
                    userMgr.AddClaimsAsync(
                        admin,
                        new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, "admin"),
                            new Claim(JwtClaimTypes.Role, "Admin")
                        }
                    ).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            if (customer == null)
            {
                customer = new User
                {
                    UserName = "customer",
                    Email = "customer@duckieswat.com",
                    EmailConfirmed = true,
                    PhoneNumber = "0123456755",
                    PhoneNumberConfirmed = true
                };
                var result = userMgr.CreateAsync(customer, "Str0ng!Passw0rd").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(customer, "Customer").Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result =
                    userMgr.AddClaimsAsync(
                        customer,
                        new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, customer.UserName),
                            new Claim(JwtClaimTypes.Role, "Customer")
                        }
                    ).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        private static void EnsureRoles(IServiceScope scope)
        {
            RoleManager<Role> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            Role admin = roleMgr.FindByNameAsync("Admin").Result;
            Role customer = roleMgr.FindByNameAsync("Customer").Result;
            if (admin == null)
            {
                admin = new Role
                {
                    Name = "Admin",
                    NormalizedName = "admin"
                };
                var result = roleMgr.CreateAsync(admin).Result;
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
                var result = roleMgr.CreateAsync(customer).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
    }

}
