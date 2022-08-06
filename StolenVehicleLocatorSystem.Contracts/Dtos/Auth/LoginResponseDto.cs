namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
