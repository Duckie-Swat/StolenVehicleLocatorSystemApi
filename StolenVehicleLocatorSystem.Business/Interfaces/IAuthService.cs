using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> Register(RegisterUserDto newUser);

        Task<LoginResponseDto> Login(LoginUserDto loginUser);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);

        Task<object> UpdateToken(string email, string oldRefreshToken, ClaimsPrincipal claimsPrincipal);

        Task<bool> RevokeToken(Guid userId);
    }
}
