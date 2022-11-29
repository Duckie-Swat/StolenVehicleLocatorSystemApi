using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Constants;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface ILostVehicleRequestService
    {
        Task<LostVehicleRequestDto> CreateAsync(CreateLostVehicleRequestDto createLostVehicleRequest);
        Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsync(LostVehicleRequestSearch filters);
        Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsyncByUserId(BaseSearch filters, Guid userId);
        Task MarkStatusAsync(Guid id, Guid userId, LostVehicleRequestStatus status);
        Task SoftRemoveOne(Guid requestId, Guid userId);
        Task SoftRemoveAll(Guid userId);
        Task HardRemoveOne(Guid requestId, Guid userId);
        Task HardRemoveAll(Guid userId);
    }
}
