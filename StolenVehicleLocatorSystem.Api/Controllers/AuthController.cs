using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Api.Controllers
{

    [ApiController]
    [Route(Endpoints.Auth)]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;


        public AuthController(ILogger<AuthController> logger,
            IAuthService authService
            )
        {
            _logger = logger;
            _authService = authService;

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
                return BadRequest("Token is expired or not valid");
            var email = principals.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null)
                return BadRequest("Claim is not valid");
            if (await _authService.IsVerify(email.Value))
                return BadRequest("This account has already verified");
            return await _authService.VerifyEmail(email.Value) ? Ok("Your email verified successful") : BadRequest("Something went wrong when verify email");
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
                return BadRequest();
            }
            return Created($"{Endpoints.Auth}/Signup", result);
        }

        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SigninAsync(LoginUserDto loginUser)
        {
            _logger.LogInformation("User login");
            LoginResponseDto response = await _authService.Login(loginUser);
            return response != null ? Ok(response) : Unauthorized();

        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = _authService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }
            string email = principal.Claims.FirstOrDefault(p => p.Type == JwtClaimTypes.Email).Value;
            try
            {
                var result = await _authService.UpdateToken(email, refreshToken, principal);
                return new ObjectResult(result);
            }
            catch (Exception)
            {

                return BadRequest("Invalid access token or refresh token");
            }
        }

        [Authorize]
        [HttpPost("revoke/{UserId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Revoke(Guid userId)
        {
            var result = await _authService.RevokeToken(userId);
            return result ? NoContent() : BadRequest("Invalid userId");
        }
    }
}
