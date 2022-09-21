using StolenVehicleLocatorSystem.Api.Middlewares;

namespace StolenVehicleLocatorSystem.Api.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseValidUserLogin(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidUserLoginMiddleware>();
        }
    }
}
