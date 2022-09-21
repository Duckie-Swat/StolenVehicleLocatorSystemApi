using Microsoft.AspNetCore.Identity;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.Net;

namespace StolenVehicleLocatorSystem.Api.Middlewares
{
    public class ValidUserLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidUserLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
            UserManager<User> userManager
            )
        {
            if (!string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                var user = await userManager.FindByNameAsync(httpContext.User.Identity.Name);
                if(user.IsDeleted)
                    throw new HttpStatusException(HttpStatusCode.Forbidden,"This user deleted. Please contact admin to solve this problem.");
            }
            
            await _next(httpContext);
        }
    }
}
