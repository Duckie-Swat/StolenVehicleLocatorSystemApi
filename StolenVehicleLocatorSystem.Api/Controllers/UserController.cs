using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Filters;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [ApiController]
    [Route(Endpoints.Users)]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("find")]
        public async Task<IActionResult> FindPagedUsersAsync([FromQuery] UserFilter filter)
        {
            return Ok(await _userService.PagedQueryAsync(filter));
        }
    }
}