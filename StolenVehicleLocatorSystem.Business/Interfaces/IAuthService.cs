using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> Register(RegisterUserDto newUser);

        Task<LoginResponseDto> Login(LoginUserDto loginUser);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);

        ClaimsPrincipal? GetPrincipalFromToken(string? token);

        Task<object> UpdateToken(string email, string oldRefreshToken, ClaimsPrincipal claimsPrincipal);

        Task<bool> RevokeToken(Guid userId);

        Task<bool> VerifyEmail(string email);

        Task<bool> IsVerify(string email);

        Task SendVerifyEmailAsync(string email);

    }
}
