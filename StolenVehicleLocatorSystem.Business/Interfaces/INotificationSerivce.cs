using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface INotificationSerivce
    {
        Task<NotificationDto> CreateAsync (CreateNotificationDto createNotificationDto);
        Task<PagedResponseModel<NotificationDto>> PagedQueryAsync(BaseFilter filters);
        Task MaskAsRead(Guid notificationId, Guid userId);
        Task MaskAllAsRead(Guid userId);
        Task SoftRemoveOne(Guid notificationId, Guid userId);
        Task SoftRemoveAll(Guid userId);
        Task HardRemoveOne(Guid notificationId, Guid userId);
        Task HardRemoveAll(Guid userId);
    }
}
