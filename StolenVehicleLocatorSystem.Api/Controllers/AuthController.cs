using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Models;
using IdentityModel;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [ApiController]
    [Route(Endpoints.Auth)]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserTokenService _userTokenService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(ILogger<AuthController> logger,
            IAuthService authService,
            IUserService userService,
            IUserTokenService userTokenService,
            ITokenService tokenService
            )
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
            _userTokenService = userTokenService;
            _tokenService = tokenService;
        }

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
                    DisplayName = user.FirstName + " " + user.LastName
                }
            });
        }

        [HttpPatch("my-account/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                throw new BadRequestException("Claim is not valid");
            await _authService.ChangePassword(email.Value, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            return NoContent();
        }

        /// <summary>
        /// Send Email to client
        /// </summary>
        /// <returns></returns>
        [HttpPost("email")]
        [Authorize]
        public async Task<IActionResult> SendVerifyEmailRequestAsync()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                return BadRequest("Claim is not valid");
            await _authService.SendVerifyEmailAsync(email.Value);
            return Ok();
        }

        /// <summary>
        /// Verify email's user
        /// </summary>
        /// <returns></returns>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync(string token)
        {
            var principals = _tokenService.GetPrincipalFromToken(token, false);
            if (principals == null)
                throw new BadRequestException("Token is expired or not valid");
            var email = principals.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                throw new BadRequestException("Claim is not valid");
            if (await _authService.IsVerify(email.Value))
                throw new BadRequestException("ClaimThis account has already verified");
            if (await _authService.VerifyEmail(email.Value))
            {
                return Ok("Your email verified successful");
            }
            throw new BadRequestException("Something went wrong when verify email");
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUserAsync(RegisterUserDto newUser)
        {
            _logger.LogInformation("Create user");
            var result = await _authService.Register(newUser);
            if (result == null)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Invalid username or password");
            }
            return Created($"{Endpoints.Auth}/Signup", result);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SigninAsync(LoginUserDto loginUser)
        {
            _logger.LogInformation("User login");
            var tokenResponse = await _authService.Login(loginUser);
            return Ok(tokenResponse);

        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new BadRequestException("Invalid client request");
            }

            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromToken(accessToken, true);

            if (principal == null)
            {
                throw new BadRequestException("Invalid access token or refresh token");
            }
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id);
            if (userIdClaim == null)
                throw new HttpStatusException(HttpStatusCode.NotFound, "This user doesn't exist");
            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new BadRequestException("UserId is not correct format");
            }

            var result = await _userTokenService.UpdateToken(userId, refreshToken, principal);
            return new ObjectResult(result);

        }

        [Authorize]
        [HttpPost("revoke/{UserId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Revoke(string refreshToken)
        {
            var result = await _userTokenService.RevokeToken(refreshToken);
            return result ? NoContent() : BadRequest("Invalid userId");
        }

        [Authorize]
        [HttpPost("revoke-all/{UserId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevokeAll(Guid userId)
        {
            var result = await _userTokenService.RevokeAllToken(userId);
            return result ? NoContent() : throw new BadRequestException("Something went wrong when verify email");
        }
    }
}
