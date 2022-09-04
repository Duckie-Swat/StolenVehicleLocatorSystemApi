using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Route(Endpoints.Cameras)]
    [ApiController]
    [Authorize]
    public class CameraController : ControllerBase
    {
        private readonly ICameraService _cameraService;
        
        public CameraController(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        /// <summary>
        /// Create new camera 
        /// </summary>
        /// <param name="newCamera"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNotification(CreateCameraDto newCamera)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
            newCamera.UserId = Guid.Parse(userId!);
            var camera = await _cameraService.CreateAsync(newCamera);
            return Created(Endpoints.Notifications, camera);
        }

        /// <summary>
        /// Search, pagination, sort cameras
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> FindPagedNotifications([FromQuery] BaseSearch filter)
        {
            return Ok(await _cameraService.PagedQueryAsync(filter));
        }
    }
}
