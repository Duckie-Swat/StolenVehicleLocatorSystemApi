using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StolenVehicleLocatorSystem.Api.Hubs;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Filters;
using System.Linq;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route(Endpoints.Notifications)]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationSerivce _notificationSerivce;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public NotificationController(INotificationSerivce notificationSerivce, 
            IHubContext<NotificationHub> notificationHubContext)
        {
            _notificationSerivce = notificationSerivce;
            _notificationHubContext = notificationHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = await _notificationSerivce.CreateAsync(createNotificationDto);
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            await _notificationHubContext.Clients.Users(email!).SendAsync("SendNotification", notification);
            return Created(Endpoints.Notifications, notification);
        }


        [HttpGet("find")]
        public async Task<IActionResult> FindPagedNotificationsAsync([FromQuery] BaseFilter filter)
        {
            return Ok(await _notificationSerivce.PagedQueryAsync(filter));
        }
    }
}
