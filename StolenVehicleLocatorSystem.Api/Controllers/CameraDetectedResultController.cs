using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StolenVehicleLocatorSystem.Api.Hubs;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Route(Endpoints.CameraDetectedResult)]
    [ApiController]
    public class CameraDetectedResultController : ControllerBase
    {
        private readonly ICameraDetectedResultService _cameraDetectedResultService;
        private readonly INotificationSerivce _notificationSerivce;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public CameraDetectedResultController(
            ICameraDetectedResultService cameraDetectedResultService,
            INotificationSerivce notificationSerivce,
            IHubContext<NotificationHub> notificationHubContext
            )
        {
            _cameraDetectedResultService = cameraDetectedResultService;
            _notificationSerivce = notificationSerivce;
            _notificationHubContext = notificationHubContext;
        }

        /// <summary>
        /// Search, pagination, sort Camera Detected Results
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> FindPagedCameraDetectedResults([FromQuery] BaseSearch filter)
        {
            return Ok(await _cameraDetectedResultService.PagedQueryAsync(filter));
        }
        /// <summary>
        /// Create new Camera Detected Result
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCameraDetectedResult(CreateCameraDetectedResultDto createCameraDetectedResultDto)
        {

            var cameraDetectedResultDto = await _cameraDetectedResultService.CreateAsync(createCameraDetectedResultDto);

            // Notify to user
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
            var createNotificationDto = new CreateNotificationDto
            {
                Title = "Found your vehicle",
                Description = $"Your vehicle has been found by camera {createCameraDetectedResultDto.CameraId}",
                UserId = Guid.Parse(userId!)
            };
            createNotificationDto.UserId = Guid.Parse(userId!);
            var notification = await _notificationSerivce.CreateAsync(createNotificationDto);
            await _notificationHubContext.Clients.Users(email!).SendAsync("SendNotification", notification);
            return Created(Endpoints.CameraDetectedResult, cameraDetectedResultDto);
        }

        /// <summary>
        /// Create new Camera Detected Result
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("List")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateListCameraDetectedResult(IEnumerable<CreateCameraDetectedResultDto> createListCameraDetectedResultDto)
        {

            var cameraDetectedResultDto = await _cameraDetectedResultService.CreateListAsync(createListCameraDetectedResultDto);
            return Created(Endpoints.CameraDetectedResult, cameraDetectedResultDto);
        }
    }
}
