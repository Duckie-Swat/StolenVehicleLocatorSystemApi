using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface ICameraService
    {
        Task<CameraDto> CreateAsync(CreateCameraDto camera);
        Task<CameraDto> GetByIdAsync(Guid id);
        Task<PagedResponseModel<CameraDto>> PagedQueryAsync(BaseSearch filters);
        Task<PagedResponseModel<CameraDto>> PagedQueryAsyncByUserId(BaseSearch filters, Guid userId);
        Task UpdateAsync(UpdateCameraDto updateCameraDto);
        Task SoftRemoveOne(Guid cameraId, Guid userId);
        Task SoftRemoveAll(Guid userId);
        Task HardRemoveOne(Guid cameraId, Guid userId);
        Task HardRemoveAll(Guid userId);
    }
}
