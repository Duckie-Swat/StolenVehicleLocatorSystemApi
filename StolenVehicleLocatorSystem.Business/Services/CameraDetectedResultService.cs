using AutoMapper;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.Extensions;
using System.Net;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class CameraDetectedResultService : ICameraDetectedResultService
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<CameraDetectedResult> _cameraDetectedResult;
        private readonly ICameraService _cameraService;

        public CameraDetectedResultService(IMapper mapper,
            IBaseRepository<CameraDetectedResult> cameraDetectedResult,
            ICameraService cameraService
            )
        {
            _mapper = mapper;
            _cameraDetectedResult = cameraDetectedResult;
            _cameraService = cameraService;
        }

        public async Task<CameraDetectedResultDto> CreateAsync(CreateCameraDetectedResultDto createCameraDetectedResultDto)
        {
            bool isCameraExist = await _cameraService.IsExist(createCameraDetectedResultDto.CameraId);
            if (!isCameraExist)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This camera is not exist");
            var cameraDetectedResult = _mapper.Map<CameraDetectedResult>(createCameraDetectedResultDto);
            return _mapper.Map<CameraDetectedResultDto>(await _cameraDetectedResult.AddAsync(cameraDetectedResult));
        }

        public async Task HardRemoveOne(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponseModel<CameraDetectedResultDto>> PagedQueryAsync(BaseSearch filter)
        {
            var query = _cameraDetectedResult.Entities;
            query = query.Where(request => string.IsNullOrEmpty(filter.Keyword)
                        || request.CameraId.ToString().Contains(filter.Keyword));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var cameraDetectedResults = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<CameraDetectedResultDto>
            {
                CurrentPage = filter.Page,
                TotalItems = cameraDetectedResults.TotalItems,
                Items = _mapper.Map<IEnumerable<CameraDetectedResultDto>>(cameraDetectedResults.Items),
                TotalPages = cameraDetectedResults.TotalPages
            };
        }

        public async Task<PagedResponseModel<CameraDetectedResultDto>> PagedQueryAsyncByCameraId(BaseSearch filter, Guid cameraId)
        {
            var query = _cameraDetectedResult.Entities;
            query = query.Where(request => ((string.IsNullOrEmpty(filter.Keyword)
                        || request.Location.Contains(filter.Keyword))
                        && request.CameraId == cameraId
                        ));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var cameraDetectedResults = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<CameraDetectedResultDto>
            {
                CurrentPage = filter.Page,
                TotalItems = cameraDetectedResults.TotalItems,
                Items = _mapper.Map<IEnumerable<CameraDetectedResultDto>>(cameraDetectedResults.Items),
                TotalPages = cameraDetectedResults.TotalPages
            };
        }

        public async Task SoftRemoveOne(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
