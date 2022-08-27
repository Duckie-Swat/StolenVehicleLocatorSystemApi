using StolenVehicleLocatorSystem.Contracts;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface INotificationSerivce
    {
        Task<NotificationDto> CreateAsync (CreateNotificationDto createNotificationDto);
        Task<PagedResponseModel<NotificationDto>> PagedQueryAsync(BaseFilter filters);
    }
}
