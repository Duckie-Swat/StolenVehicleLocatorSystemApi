using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailDto> Register(RegisterUserDto newUser, string role);

        Task<UserDetailDto> GetByEmail(string email);

        Task<PagedResponseModel<UserDetailDto>> PagedQueryAsync(UserSearch filters);

        Task UpdateUserAsync(string email, UpdateUserDto updateUserRequest, Guid updateBy);

        Task RestoreOneAsync(Guid id);

        Task RestoreManyAsync(Guid[] ids);

        Task SoftRemoveOne(Guid userId);
        Task SoftRemoveMany(Guid[] userIds);

        Task HardRemoveOne(Guid userId);

        Task HardRemoveMany(Guid[] userIds);

    }
}
