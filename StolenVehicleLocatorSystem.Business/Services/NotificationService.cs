using AutoMapper;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class NotificationService : INotificationSerivce
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Notification> _notification;

        public NotificationService(IMapper mapper, IBaseRepository<Notification> notification)
        {
            _mapper = mapper;
            _notification = notification;
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto createNotificationDto)
        {
            var notication = _mapper.Map<Notification>(createNotificationDto);
            return _mapper.Map<NotificationDto>(await _notification.AddAsync(notication));
        }

        public async Task<PagedResponseModel<NotificationDto>> PagedQueryAsync(BaseFilter filter)
        {
            var query = _notification.Entities;
            query = query.Where(notification => string.IsNullOrEmpty(filter.Keyword)
                        || notification.Title.ToString().Contains(filter.Keyword));

            if (!string.IsNullOrEmpty(filter.OrderProperty) && filter.Desc != null)
            {
                query = query.OrderByPropertyName(filter.OrderProperty, (bool)filter.Desc);
            }

            var notifications = await query.PaginateAsync(filter.Page, filter.Limit);
            return new PagedResponseModel<NotificationDto>
            {
                CurrentPage = filter.Page,
                TotalItems = notifications.TotalItems,
                Items = _mapper.Map<IEnumerable<NotificationDto>>(notifications.Items),
                TotalPages = notifications.TotalPages
            };
        }
    }
}
