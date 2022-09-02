using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
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
        private readonly ICameraService _cameraService;

        public UserController(IUserService userService, UserManager<User> userManager, 
            INotificationSerivce notificationSerivce, ICameraService cameraService)
        {
            _userService = userService;
            _userManager = userManager;
            _notificationSerivce = notificationSerivce;
            _cameraService = cameraService;
        }

        /// <summary>
        /// Search, pagination, sort users
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("find")]
        [Authorize(Roles = $"{RoleTypes.Admin}")]
        public async Task<IActionResult> FindPagedUsersAsync([FromQuery] UserSearch filter)
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

        [HttpGet("my-account/notifications/find")]
        [Authorize]
        public async Task<IActionResult> FindPagedNotificationsFromCurrentUser([FromQuery] BaseSearch filter)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            return Ok(await _notificationSerivce.PagedQueryAsyncByUserId(filter, currentUserId));
        }

        /// <summary>
        /// Mask a notification of current user as read based on notification id 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="updateUserRequest"></param>
        /// <returns></returns>
        [HttpPatch("my-account/notifications/{notificationId}/mask")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MaskNotificationAsReadByIdFromCurrentUser(Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.MaskAsRead(notificationId, currentUserId);
            return NoContent();
        }
        /// <summary>
        /// Mask all notification as read based on  user id 
        /// </summary>
        /// <returns></returns>
        [HttpPatch("my-account/notifications/mask")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MaskNotificationsAsReadFromCurrentUser()
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.MaskAllAsRead(currentUserId);
            return NoContent();
        }
        /// <summary>
        /// Soft remove a notification as read based on notification id and user id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpDelete("my-account/notifications/{notificationId}/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftRemoveNotificationByIdFromCurrentUser(Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.SoftRemoveOne(notificationId, currentUserId);
            return NoContent();
        }

        /// <summary>
        ///  Soft remove all notification as read based on  user id 
        /// </summary>
        /// <returns></returns>
        [HttpDelete("my-account/notifications/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftRemoveNotificationsByUserId()
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.SoftRemoveAll(currentUserId);
            return NoContent();
        }
        /// <summary>
        /// Hard remove a notification as read based on notification id and user id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpDelete("my-account/notifications/{notificationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardRemoveNotificationById(Guid notificationId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.HardRemoveOne(notificationId, currentUserId);
            return NoContent();
        }
        /// <summary>
        /// Hard remove all notification as read based on  user id 
        /// </summary>
        /// <returns></returns>
        [HttpDelete("my-account/cameras/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardRemoveNotificationsByUserId()
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _notificationSerivce.HardRemoveAll(currentUserId);
            return NoContent();
        }

        /// <summary>
        /// List cameras of current user
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("my-account/cameras/find")]
        [Authorize]
        public async Task<IActionResult> FindPagedCamerasFromCurrentUser([FromQuery] BaseSearch filter)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            return Ok(await _cameraService.PagedQueryAsyncByUserId(filter, currentUserId));
        }
        /// <summary>
        /// Update camera by id from current user
        /// </summary>
        /// <param name="updateCameraDto"></param>
        /// <returns></returns>
        [HttpPatch("my-account/cameras/{cameraId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCameraByIdFromCurrentUser(UpdateCameraDto updateCameraDto)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            updateCameraDto.UserId = currentUserId;
            await _cameraService.UpdateAsync(updateCameraDto);
            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraId"></param>
        /// <returns></returns>
        [HttpDelete("my-account/cameras/{cameraId}/soft")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftRemoveCameraByIdFromCurrentUser(Guid cameraId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value!);
            await _cameraService.SoftRemoveOne(cameraId, currentUserId);
            return NoContent();
        }

        
    }
}