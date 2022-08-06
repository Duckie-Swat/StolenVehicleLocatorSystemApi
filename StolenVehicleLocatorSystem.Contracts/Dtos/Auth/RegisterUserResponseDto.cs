using StolenVehicleLocatorSystem.Contracts.Dtos.User;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class RegisterUserResponseDto
    {
        public UserDetailDto Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
