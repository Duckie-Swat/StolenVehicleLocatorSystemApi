namespace StolenVehicleLocatorSystem.Contracts.Models
{

    public class VerifyCaptchaModel
    {
        public string Secret { get; set; }
        public string Response { get; set; }
        public string? RemoteIp { get; set; } = "0.0.0.0";
    }
}
