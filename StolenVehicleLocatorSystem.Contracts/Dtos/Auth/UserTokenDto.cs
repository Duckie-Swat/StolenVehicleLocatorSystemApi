namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class UserTokenDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Platform { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
