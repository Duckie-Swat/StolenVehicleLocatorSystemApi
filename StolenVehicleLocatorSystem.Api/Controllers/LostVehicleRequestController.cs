using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [Route(Endpoints.LostVehicleRequest)]
    [ApiController]
    [Authorize(Roles = RoleTypes.Admin)]
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
        public async Task<IActionResult> FindPagedLostVehicleRequests([FromQuery] BaseSearch filter)
        {
            return Ok(await _lostVehicleRequestService.PagedQueryAsync(filter));
        }
    }
}
