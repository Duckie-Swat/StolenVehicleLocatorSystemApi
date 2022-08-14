using StolenVehicleLocatorSystem.Contracts.Models;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IMailKitEmailService
    {
        Task SendWelcomeEmailAsync(WelcomeRequest request, string filePathTemplate);
    }
}
