using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IUserTokenService
    {
        Task<bool> CreateUserToken(CreateUserTokenDto createUserTokenDto);
        Task<bool> UpdateUserToken(string refreshToken);
        Task<bool> RevokeToken(string refreshToken);
        Task<bool> RevokeAllToken(Guid userId);
    }
}
