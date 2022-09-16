using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Route(Endpoints.LostVehicleRequest)]
    [ApiController]
    [Authorize]
    public class LostVehicleRequestController : ControllerBase
    {
        private readonly ILostVehicleRequestService _lostVehicleRequestService;

        public LostVehicleRequestController(ILostVehicleRequestService lostVehicleRequestService)
        {
            _lostVehicleRequestService = lostVehicleRequestService;
        }

        /// <summary>
        /// Search, pagination, sort lost vehicle requests
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> FindPagedLostVehicleRequests([FromQuery] BaseSearch filter)
        {
            return Ok(await _lostVehicleRequestService.PagedQueryAsync(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createLostVehicleRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLostVehicleRequest(CreateLostVehicleRequestDto createLostVehicleRequestDto)
        {
            await _lostVehicleRequestService.CreateAsync(createLostVehicleRequestDto);
            return Created(Endpoints.LostVehicleRequest, null);
        }
    }
}
