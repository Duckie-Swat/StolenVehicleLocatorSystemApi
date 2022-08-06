using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rookie.Ecom.MetaShop.Contracts.Constants;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.DataAccessor.Models;
using System.IdentityModel.Tokens.Jwt;
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

        [HttpPost("Signup")]
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

        [HttpPost("Signin")]
        public async Task<IActionResult> SigninAsync(LoginUserDto loginUser)
        {
            _logger.LogInformation("User login");
            LoginResponseDto response = await _authService.Login(loginUser);
            if (response != null)
            {
                return Ok(response);
            }
            return Unauthorized();
        }
    }
}
