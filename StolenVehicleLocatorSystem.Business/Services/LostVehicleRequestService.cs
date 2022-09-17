using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Constants;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.Extensions;
using System.Net;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class LostVehicleRequestService : ILostVehicleRequestService
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<LostVehicleRequest> _lostVehicleRequest;

        public LostVehicleRequestService(IMapper mapper, IBaseRepository<LostVehicleRequest> lostVehicleRequest)
        {
            _mapper = mapper;
            _lostVehicleRequest = lostVehicleRequest;
        }

        public async Task<LostVehicleRequestDto> CreateAsync(CreateLostVehicleRequestDto createLostVehicleRequest)
        {
            var query = _lostVehicleRequest.Entities;
            if(query.Any(req => req.PlateNumber == createLostVehicleRequest.PlateNumber && req.Status == LostVehicleRequestStatus.PROCESSING))
                throw new BadRequestException("This vehicle has already procced");
            var newLostVehicleRequest = _mapper.Map<LostVehicleRequest>(createLostVehicleRequest);
            return _mapper.Map<LostVehicleRequestDto>(await _lostVehicleRequest.AddAsync(newLostVehicleRequest));
        }

        public async Task HardRemoveAll(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task HardRemoveOne(Guid requestId, Guid userId)
        {
            var lostRequestVehicle = await _lostVehicleRequest.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == requestId);
            if (lostRequestVehicle == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This request doesn't exist");
            await _lostVehicleRequest.DeleteAsync(lostRequestVehicle.Id);
        }

        public async Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsync(BaseSearch filter)
        {
            var query = _lostVehicleRequest.Entities;
            query = query.Where(request => string.IsNullOrEmpty(filter.Keyword)
                        || request.PlateNumber.Contains(filter.Keyword));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var lostVehicleRequests = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<LostVehicleRequestDto>
            {
                CurrentPage = filter.Page,
                TotalItems = lostVehicleRequests.TotalItems,
                Items = _mapper.Map<IEnumerable<LostVehicleRequestDto>>(lostVehicleRequests.Items),
                TotalPages = lostVehicleRequests.TotalPages
            };
        }

        public async Task<PagedResponseModel<LostVehicleRequestDto>> PagedQueryAsyncByUserId(BaseSearch filter, Guid userId)
        {
            var query = _lostVehicleRequest.Entities;
            query = query.Where(request => ((string.IsNullOrEmpty(filter.Keyword)
                        || request.PlateNumber.Contains(filter.Keyword))
                        && request.UserId == userId
                        ));
            // filter
            query = query.Where(user => filter.IsDeleted == null
                        || user.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var lostVehicleRequests = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<LostVehicleRequestDto>
            {
                CurrentPage = filter.Page,
                TotalItems = lostVehicleRequests.TotalItems,
                Items = _mapper.Map<IEnumerable<LostVehicleRequestDto>>(lostVehicleRequests.Items),
                TotalPages = lostVehicleRequests.TotalPages
            };
        }

        public async Task SoftRemoveAll(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task SoftRemoveOne(Guid requestId, Guid userId)
        {
            var lostVehicleRequest = await _lostVehicleRequest.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == requestId);
            if (lostVehicleRequest == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This request doesn't exist");
            lostVehicleRequest.IsDeleted = true;
            lostVehicleRequest.LastUpdatedAt = DateTime.UtcNow;
            lostVehicleRequest.LastUpdatedBy = userId;
            await _lostVehicleRequest.UpdateAsync(lostVehicleRequest);
        }
    }
}
