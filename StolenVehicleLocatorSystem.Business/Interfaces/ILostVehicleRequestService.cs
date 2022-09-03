using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface ILostVehicleRequestService
    {
        Task<LostVehicleRequestDto> CreateAsync(CreateLostVehicleRequestDto createLostVehicleRequest);
        Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsync(BaseSearch filters);
        Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsyncByUserId(BaseSearch filters, Guid userId);
        Task SoftRemoveOne(Guid requestId, Guid userId);
        Task SoftRemoveAll(Guid userId);
        Task HardRemoveOne(Guid requestId, Guid userId);
        Task HardRemoveAll(Guid userId);
    }
}
