﻿using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface ICameraDetectedResultService
    {
        Task<CameraDetectedResultDto> CreateAsync(CreateCameraDetectedResultDto createCameraDetectedResultDto);
        Task<PagedResponseModel<CameraDetectedResultDto>> PagedQueryAsync(BaseSearch filters);
        Task<PagedResponseModel<CameraDetectedResultDto>> PagedQueryAsyncByCameraId(BaseSearch filters, Guid cameraId);
        Task SoftRemoveOne(Guid id);
        Task HardRemoveOne(Guid id);
    }
}