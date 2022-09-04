using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Route(Endpoints.CameraDetectedResult)]
    [ApiController]
    [Authorize]
    public class CameraDetectedResultController : ControllerBase
    {
        private readonly ICameraDetectedResultService _cameraDetectedResultService;

        public CameraDetectedResultController(ICameraDetectedResultService cameraDetectedResultService)
        {
            _cameraDetectedResultService = cameraDetectedResultService;
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
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCameraDetectedResult([FromQuery] BaseSearch filter)
        {
            return Created(Endpoints.CameraDetectedResult, null);
        }
    }
}
