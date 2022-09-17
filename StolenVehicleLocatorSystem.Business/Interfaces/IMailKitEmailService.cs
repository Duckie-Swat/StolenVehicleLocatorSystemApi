using StolenVehicleLocatorSystem.Contracts.Models;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IMailKitEmailService
    {
        Task SendWelcomeEmailAsync(WelcomeResponse res, string filePathTemplate);
        Task SendResetPasswordEmailAsync(ResetEmailResponse res, string filePathTemplate);
    }
}
