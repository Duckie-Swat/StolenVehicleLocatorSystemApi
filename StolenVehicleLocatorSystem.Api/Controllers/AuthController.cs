using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net;
using StolenVehicleLocatorSystem.Contracts.Exceptions;

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


        public AuthController(ILogger<AuthController> logger,
            IAuthService authService,
            IUserService userService,
            IUserTokenService userTokenService
            )
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
            _userTokenService = userTokenService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                throw new BadRequestException("Claim is not valid");
            var user = await _userService.GetByEmail(email.Value);
            if (user == null)
                return NotFound();
            return Ok(user);
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
            var principals = _authService.GetPrincipalFromToken(token);
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



        [HttpPost("signup")]
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

        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SigninAsync(LoginUserDto loginUser)
        {
            _logger.LogInformation("User login");
            return Ok(await _authService.Login(loginUser));

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

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = _authService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Invalid access token or refresh token");
            }
            string email = principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email).Value;

            var result = await _authService.UpdateToken(email, refreshToken, principal);
            return new ObjectResult(result);

        }

        [Authorize]
        [HttpPost("revoke/{UserId}")]
        public async Task<IActionResult> Revoke(string refreshToken)
        {
            var result = await _userTokenService.RevokeToken(refreshToken);
            return result ? NoContent() : BadRequest("Invalid userId");
        }

        [Authorize]
        [HttpPost("revoke-all/{UserId}")]
        public async Task<IActionResult> RevokeAll(Guid userId)
        {
            var result = await _userTokenService.RevokeAllToken(userId);
            return result ? NoContent() : throw new BadRequestException("Something went wrong when verify email");
        }
    }
}
