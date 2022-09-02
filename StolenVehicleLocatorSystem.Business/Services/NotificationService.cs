using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StolenVehicleLocatorSystem.Business.Extensions;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.Extensions;
using System.Net;

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

        public async Task HardRemoveAll(Guid userId)
        {
            var notifications = _notification.Entities.Where(x => x.UserId == userId);
            if (!notifications.Any())
                throw new HttpStatusException(HttpStatusCode.NotFound, "Notifcations is empty");
            await _notification.RemoveRangeAsync(notifications);
        }

        public async Task HardRemoveOne(Guid notificationId, Guid userId)
        {
            var notication = await _notification.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == notificationId);
            if (notication == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This notification doesn't exist");
            await _notification.DeleteAsync(notication.Id);
        }

        public async Task MaskAllAsRead(Guid userId)
        {
            var notifications = _notification.Entities.Where(x => x.UserId == userId);
            if(!notifications.Any())
                throw new HttpStatusException(HttpStatusCode.NotFound, "Notifcations is empty");
            var notificationList = await notifications.ToListAsync();
            notificationList.ForEach(n =>
            {
                n.IsUnRead = false;
                n.LastUpdatedAt = DateTime.UtcNow;
                n.LastUpdatedBy = userId;
            });
            await _notification.UpdateRangeAsync(notificationList);
        }

        public async Task MaskAsRead(Guid notificationId, Guid userId)
        {
            var notication = await _notification.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == notificationId);
            if (notication == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This notification doesn't exist");
            notication.IsUnRead = false;
            notication.LastUpdatedAt = DateTime.UtcNow;
            notication.LastUpdatedBy = userId;
            await _notification.UpdateAsync(notication);
        }

        public async Task<PagedResponseModel<NotificationDto>> PagedQueryAsync(BaseSearch filter)
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

        public async Task<PagedResponseModel<NotificationDto>> PagedQueryAsyncByUserId(BaseSearch filter, Guid userId)
        {
            var query = _notification.Entities;
            query = query.Where(notification => ((string.IsNullOrEmpty(filter.Keyword)
                        || notification.Title.ToString().Contains(filter.Keyword) ) 
                        && notification.UserId == userId
                        ));

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

        public async Task SoftRemoveAll(Guid userId)
        {
            var notifications = _notification.Entities.Where(x => x.UserId == userId);
            if (!notifications.Any())
                throw new HttpStatusException(HttpStatusCode.NotFound, "Notifcations is empty");
            var notificationList = await notifications.ToListAsync();
            notificationList.ForEach(n =>
            {
                n.IsDeleted = true;
                n.LastUpdatedAt = DateTime.UtcNow;
                n.LastUpdatedBy = userId;
            });
            await _notification.UpdateRangeAsync(notificationList);
        }

        public async Task SoftRemoveOne(Guid notificationId, Guid userId)
        {
            var notication = await _notification.Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == notificationId);
            if (notication == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This notification doesn't exist");
            notication.IsDeleted = true;
            notication.LastUpdatedAt = DateTime.UtcNow;
            notication.LastUpdatedBy = userId;
            await _notification.UpdateAsync(notication);
        }
    }
}
