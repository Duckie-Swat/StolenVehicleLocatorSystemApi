namespace StolenVehicleLocatorSystem.Contracts.Models
{
    public class WelcomeRequest : BaseEmailRequest
    {
        public string VerifyEmailUrl { get; set; }
    }
}
