using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StolenVehicleLocatorSystem.Api.Hubs;
using StolenVehicleLocatorSystem.Business.Interfaces;  
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
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
        /// <summary>
        /// Create new notification and notify new notification to all online devices
        /// </summary>
        /// <param name="createNotificationDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            var notification = await _notificationSerivce.CreateAsync(createNotificationDto);
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            await _notificationHubContext.Clients.Users(email!).SendAsync("SendNotification", notification);
            return Created(Endpoints.Notifications, notification);
        }

        /// <summary>
        /// Search, pagination, sort notification
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        public async Task<IActionResult> FindPagedNotifications([FromQuery] BaseFilter filter)
        {
            return Ok(await _notificationSerivce.PagedQueryAsync(filter));
        }
    }
}
