using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<AuthService> logger, SignInManager<User> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims, string secretKey)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var token = new JwtSecurityToken(
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(60),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }



        public async Task<LoginResponseDto> Login(LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.IssuedAt, DateTime.UtcNow.ToString())
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(JwtClaimTypes.Role, userRole));
                }
                var jwtSecurityToken = GetToken(authClaims, _configuration["Jwt:Secret"]);
                string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                user.AddUserToken(new IdentityUserToken<Guid>
                {
                    LoginProvider = "Local",
                    UserId = user.Id,
                    Value = token.ToString()
                });
                _userManager.UpdateAsync(user);
                return new LoginResponseDto
                {
                    Token = token,
                    Expiration = jwtSecurityToken.ValidTo
                };
            }
            return null;
        }

        public async Task<RegisterUserResponseDto> Register(RegisterUserDto newUser)
        {
            string role = "Customer";
            var userCheck = await _userManager.FindByEmailAsync(newUser.Email);
            var roleCheck = await _roleManager.FindByNameAsync(role);
            if (userCheck != null)
            {
                _logger.LogError("Account exist");
                return null;
            }
            else if (roleCheck == null)
            {
                _logger.LogError("Role doesn't exist");
                return null;
            }
            else
            {

                var user = _mapper.Map<User>(newUser);
                user.UserName = user.NormalizedUserName = user.Email;
                var newUserResult = await _userManager.CreateAsync(user, newUser.Password);
                List<string> errors = new();
                if (newUserResult.Errors.Any())
                {
                    foreach (var error in newUserResult.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    return new RegisterUserResponseDto
                    {
                        Data = null,
                        Errors = errors
                    };
                }

                var results = await Task.WhenAll(new[]
                {
                  _userManager.AddToRoleAsync(user, role),
                   _userManager.AddClaimsAsync(
                    user,
                    new Claim[]
                    {
                            new Claim(JwtClaimTypes.Email, user.Email),
                            new Claim(JwtClaimTypes.Role, role)
                    }
                )});

                return new RegisterUserResponseDto
                {
                    Data = _mapper.Map<UserDetailDto>(user),
                    Errors = null
                };

            }
        }
    }
}
