using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Models;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> Register(RegisterUserDto newUser);

        Task<TokenResponse> Login(LoginUserDto loginUser);

        Task ChangePassword(string email, string oldPassword, string newPassword);

        Task<bool> VerifyEmail(string email);

        Task<bool> IsVerify(string email);

        Task SendVerifyEmailAsync(string email);

        Task SendResetPasswordAsync(string email);

        Task ResetPassword(string token, string email, string password);

        Task<bool> IsRefreshTokenValid(string refreshToken, Guid userId);
    }
}
