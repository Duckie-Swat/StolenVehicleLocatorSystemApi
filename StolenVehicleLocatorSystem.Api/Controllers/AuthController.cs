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
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;


        public AuthController(ILogger<AuthController> logger,
            IAuthService authService,
            UserManager<User> userManager,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _authService = authService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> CreateUserAsync(RegisterUserDto newUser)
        {
            _logger.LogInformation("Create user");
            var result = await _authService.Register(newUser);
            return Created($"{Endpoints.Auth}/Signup", result);
        }

        [HttpPost("Signin")]
        public async Task<IActionResult> CreateUserAsync(LoginUserDto loginUser)
        {
            _logger.LogInformation("User login");
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = _authService.GetToken(authClaims, _configuration["Jwt:Secret"]);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
