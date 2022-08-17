using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Models;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IUserTokenService
    {
        Task<bool> CreateUserToken(CreateUserTokenDto createUserTokenDto);
        Task<object> UpdateToken(Guid userId, string oldRefreshToken, ClaimsPrincipal claimsPrincipal);
        Task<bool> RevokeToken(string refreshToken);
        Task<bool> RevokeAllToken(Guid userId);
        Task<UserTokenDto> GetByRefreshToken(string refreshToken);
    }
}
