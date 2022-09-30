using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Business.Services;
using StolenVehicleLocatorSystem.DataAccessor;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace StolenVehicleLocatorSystem.Business
{
    public static class ServiceRegister
    {
        public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccessLayer(configuration);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IUserTokenService, UserTokenService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<INotificationSerivce, NotificationService>();
            services.AddTransient<ICameraService, CameraService>();
            services.AddTransient<ILostVehicleRequestService, LostVehicleRequestService>();
            services.AddTransient<ICameraDetectedResultService, CameraDetectedResultService>();
            services.AddHttpClient();

            services.Configure<MailKitEmailSenderOptions>(options =>
            {
                options.HostAddress = configuration["ExternalProviders:MailKit:SMTP:Address"];
                options.HostPort = Convert.ToInt32(configuration["ExternalProviders:MailKit:SMTP:Port"]);
                options.HostUsername = configuration["ExternalProviders:MailKit:SMTP:Account"];
                options.HostPassword = configuration["ExternalProviders:MailKit:SMTP:Password"];
                options.SenderEmail = configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
                options.SenderName = configuration["ExternalProviders:MailKit:SMTP:SenderName"];
            });
            services.AddScoped<IMailKitEmailService, MailKitEmailSenderService>();
            services.AddScoped<IEmailSender, MailKitEmailSenderService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISecurityTokenValidator, JwtSecurityTokenHandler>();

        }
    }
}
