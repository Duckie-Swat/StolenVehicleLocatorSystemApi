namespace StolenVehicleLocatorSystem.Contracts.Models
{
    public class ResetEmailResponse : BaseEmailResponse
    {
        public string ResetPasswordUrl { get; set; }
        public string NewPassword { get; set; }
    }
}
