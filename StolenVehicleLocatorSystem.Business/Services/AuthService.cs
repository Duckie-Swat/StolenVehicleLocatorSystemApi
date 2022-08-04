using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
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

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<AuthService> logger, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
        }

        public JwtSecurityToken GetToken(List<Claim> authClaims, string secretKey)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        public async Task Login(LoginUserDto loginUser)
        {
            var user = _mapper.Map<User>(loginUser);
            var tmp = await _signInManager.CanSignInAsync(user);
            var test = 1;
        }

        public async Task<UserDetailDto> Register(RegisterUserDto newUser)
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
                user.PhoneNumberConfirmed = user.EmailConfirmed = true;
                user.UserName = user.NormalizedUserName = user.Email;

                var result = await _userManager.CreateAsync(user, newUser.Password);

                result = _userManager.AddToRoleAsync(user, role).Result;

                result = await
                _userManager.AddClaimsAsync(
                    user,
                    new Claim[]
                    {
                            new Claim(JwtClaimTypes.Email, user.Email),
                            new Claim(JwtClaimTypes.Role, role)
                    }
                );
                return _mapper.Map<UserDetailDto>(user);
            }
        }
    }
}
