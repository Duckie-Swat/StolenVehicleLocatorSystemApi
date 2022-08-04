using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailDto> Register(RegisterUserDto newUser, string role);

        Task<UserDetailDto> GetById(string id);

        Task<PagedResponseModel<UserDetailDto>> PagedQueryAsync(string username, int page, int limit);

        Task<int> CountAsync();
    }
}
