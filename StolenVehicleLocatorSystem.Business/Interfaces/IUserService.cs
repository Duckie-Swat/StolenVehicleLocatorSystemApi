using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailDto> Register(RegisterUserDto newUser, string role);

        Task<UserDetailDto> GetById(string id);

        Task<PagedResponseModel<UserDetailDto>> PagedQueryAsync(UserFilter filters);

        Task<int> CountAsync();
    }
}
