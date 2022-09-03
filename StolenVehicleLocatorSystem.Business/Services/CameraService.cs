using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.Extensions;
using System.Net;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class CameraService : ICameraService
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Camera> _cameras;

        public CameraService(IMapper mapper, IBaseRepository<Camera> camera)
        {
            _mapper = mapper;
            _cameras = camera;
        }

        public async Task<CameraDto> CreateAsync(CreateCameraDto newCamera)
        {
            var camera = _mapper.Map<Camera>(newCamera);
            return _mapper.Map<CameraDto>(await _cameras.AddAsync(camera));
        }

        public async Task<CameraDto> GetByIdAsync(Guid id)
        {
            var camera = await _cameras.GetByIdAsync(id);
            if(camera == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "Camera doesn't exist");
            return _mapper.Map<CameraDto>(camera);
        }

        public async Task HardRemoveAll(Guid userId)
        {
            var cameras = _cameras.Entities.Where(x => x.UserId == userId);
            if (!cameras.Any())
                throw new HttpStatusException(HttpStatusCode.NotFound, "Cameras is empty");
            await _cameras.RemoveRangeAsync(cameras);
        }

        public async Task HardRemoveOne(Guid cameraId, Guid userId)
        {
            var camera = await _cameras.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == cameraId);
            if (camera == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This camera doesn't exist");
            await _cameras.DeleteAsync(camera.Id);
        }

        public async Task<PagedResponseModel<CameraDto>> PagedQueryAsync(BaseSearch filter)
        {
            var query = _cameras.Entities;
            query = query.Where(camera => string.IsNullOrEmpty(filter.Keyword)
                        || camera.Name.Contains(filter.Keyword));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var cameras = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<CameraDto>
            {
                CurrentPage = filter.Page,
                TotalItems = cameras.TotalItems,
                Items = _mapper.Map<IEnumerable<CameraDto>>(cameras.Items),
                TotalPages = cameras.TotalPages
            };
        }

        public async Task<PagedResponseModel<CameraDto>> PagedQueryAsyncByUserId(BaseSearch filter, Guid userId)
        {
            var query = _cameras.Entities;
            query = query.Where(camera => ((string.IsNullOrEmpty(filter.Keyword)
                        || camera.Name.Contains(filter.Keyword)) 
                        && camera.UserId == userId
                        ));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var cameras = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<CameraDto>
            {
                CurrentPage = filter.Page,
                TotalItems = cameras.TotalItems,
                Items = _mapper.Map<IEnumerable<CameraDto>>(cameras.Items),
                TotalPages = cameras.TotalPages
            };
        }

        public async Task SoftRemoveAll(Guid userId)
        {
            var cameras = _cameras.Entities.Where(x => x.UserId == userId);
            if (!cameras.Any())
                throw new HttpStatusException(HttpStatusCode.NotFound, "Cameras is empty");
            var cameraList = await cameras.ToListAsync();
            cameraList.ForEach(n =>
            {
                n.IsDeleted = true;
                n.LastUpdatedAt = DateTime.UtcNow;
                n.LastUpdatedBy = userId;
            });
            await _cameras.UpdateRangeAsync(cameraList);
        }

        public async Task SoftRemoveOne(Guid cameraId, Guid userId)
        {
            var camera = await _cameras.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == cameraId);
            if (camera == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This camera doesn't exist");
            camera.IsDeleted = true;
            camera.LastUpdatedAt = DateTime.UtcNow;
            camera.LastUpdatedBy = userId;
            await _cameras.UpdateAsync(camera);
        }

        public async Task UpdateAsync(UpdateCameraDto updateCameraDto)
        {
            var camera = await _cameras.Entities.FirstOrDefaultAsync(c => c.Id == updateCameraDto.Id && c.UserId == updateCameraDto.UserId);
            if (camera == null)
                throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "Camera doesn't exist");
            camera.Name = updateCameraDto.Name;
            camera.LastUpdatedAt = DateTime.UtcNow;
            await _cameras.UpdateAsync(camera);
        }
    }
}
