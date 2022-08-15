namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class CreateUserTokenDto
    {
        public Guid UserId { get; set; }
        public string Platform { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
