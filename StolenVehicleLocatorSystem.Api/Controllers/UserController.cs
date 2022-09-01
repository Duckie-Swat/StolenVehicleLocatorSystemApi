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
        private readonly INotificationSerivce _notificationSerivce;

        public UserController(IUserService userService, 
            UserManager<User> userManager, 
            INotificationSerivce notificationSerivce)
        {
            _userService = userService;
            _userManager = userManager;
            _notificationSerivce = notificationSerivce;
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
        /// <summary>
        /// Mask a notification as read based on notification id and user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpPatch("{userId}/notifications/{notificationId}/mask")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MaskNotificationAsReadById(Guid userId, Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.MaskAsRead(notificationId, userId);
            return NoContent();
        }
        /// <summary>
        /// Mask all notification as read based on  user id 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpPatch("{userId}/notifications/mask")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MaskNotificationsAsReadByUserId(Guid userId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.MaskAllAsRead(userId);
            return NoContent();
        }
        /// <summary>
        /// Soft remove a notification as read based on notification id and user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/notifications/{notificationId}/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftRemoveNotificationById(Guid userId, Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.SoftRemoveOne(notificationId, userId);
            return NoContent();
        }

        /// <summary>
        ///  Soft remove all notification as read based on  user id 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/notifications/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftRemoveNotificationsByUserId(Guid userId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.SoftRemoveAll(userId);
            return NoContent();
        }
        /// <summary>
        /// Hard remove a notification as read based on notification id and user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/notifications/{notificationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardRemoveNotificationById(Guid userId, Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.HardRemoveOne(notificationId, userId);
            return NoContent();
        }
        /// <summary>
        /// Hard remove all notification as read based on  user id 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/notifications")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardRemoveNotificationsByUserId(Guid userId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            if (currentUserId != userId)
                throw new BadRequestException("You cannot perform this action");
            await _notificationSerivce.HardRemoveAll(userId);
            return NoContent();
        }
    }
}