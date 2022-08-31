using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Filters;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [ApiController]
    [Route(Endpoints.Users)]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }


        /// <summary>
        /// Search, pagination, sort users
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        public async Task<IActionResult> FindPagedUsersAsync([FromQuery] UserFilter filter)
        {
            return Ok(await _userService.PagedQueryAsync(filter));
        }
        /// <summary>
        /// Get current profile
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        [HttpGet("my-account")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                throw new BadRequestException("Claim is not valid");
            var user = await _userService.GetByEmail(email.Value);
            if (user == null)
                return NotFound();
            return new ObjectResult(new
            {
                User = new
                {
                    Id = user.Id,
                    DisplayName = user.FirstName + " " + user.LastName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = email.Value,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth
                }
            });
        }
        /// <summary>
        /// Update current profile
        /// </summary>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        [HttpPut("my-account")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateAccountInformation(UpdateUserDto updateUserRequest)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                throw new BadRequestException("Claim is not valid");
            var user = await _userService.GetByEmail(email.Value);
            if (user == null)
                return NotFound();
            await _userService.UpdateUserAsync(email.Value, updateUserRequest, user.Id);
            return NoContent();
        }
        /// <summary>
        /// Update user with specific id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpPut("{UserId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateOneUser(Guid userId, UpdateUserDto updateUserRequest)
        {
            var userToUpdate = await _userManager.FindByIdAsync(userId.ToString());
            var currentUser = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
            if (userToUpdate == null || currentUser != null)
                return NotFound();
            await _userService.UpdateUserAsync(userToUpdate.Email, updateUserRequest, Guid.Parse(currentUser));
            return NoContent();
        }
    }
}