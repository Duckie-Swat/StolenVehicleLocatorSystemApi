using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> Register(RegisterUserDto newUser);

        Task<LoginResponseDto> Login(LoginUserDto loginUser);

    }
}
