using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Models;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> Register(RegisterUserDto newUser);

        Task<TokenResponse> Login(LoginUserDto loginUser);

        Task ChangePassword(string email, string oldPassword, string newPassword);

        Task<bool> VerifyEmail(string email);

        Task<bool> IsVerify(string email);

        Task SendVerifyEmailAsync(string email);

    }
}
