using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Models;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IMailKitEmailService _mailKitEmailService;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IEmailSender emailSender,
            IMailKitEmailService mailKitEmailService
            )
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _emailSender = emailSender;
            _mailKitEmailService = mailKitEmailService;
        }

        private JwtSecurityToken CreateToken(IList<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"]
            };

            var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"]
            };
            var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
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
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(JwtClaimTypes.Role, userRole));
                }


                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);

                return new LoginResponseDto
                {
                    Token = _jwtSecurityTokenHandler.WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };

            }

            throw new BadRequestException("username or password is not correct");
        }

        public async Task<RegisterUserResponseDto> Register(RegisterUserDto newUser)
        {
            string role = "Customer";
            var userCheck = await _userManager.FindByEmailAsync(newUser.Email);
            var roleCheck = await _roleManager.FindByNameAsync(role);
            if (userCheck != null)
            {
                throw new BadRequestException("Account exist");
            }
            else if (roleCheck == null)
            {
                _ = await _roleManager.CreateAsync(new Role
                {
                    Name = role
                });
            }

            var user = _mapper.Map<User>(newUser);
            user.UserName = user.Email;
            var newUserResult = await _userManager.CreateAsync(user, newUser.Password);

            if (newUserResult.Errors.Any())
            {
                throw new BadRequestException(newUserResult.Errors.ToString());
            }

            var authClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.Role, role)
                };

            await _userManager.AddToRoleAsync(user, role);
            await _userManager.AddClaimsAsync(user, authClaims);
            await SendVerifyEmailAsync(user.Email);

            return new RegisterUserResponseDto
            {
                Data = _mapper.Map<UserDetailDto>(user),
                Errors = null
            };

        }

        public async Task SendVerifyEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var authClaims = await _userManager.GetClaimsAsync(user);

            var token = CreateToken(authClaims);

            var emailSubject = "Verify Your Email";
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
            await _mailKitEmailService.SendWelcomeEmailAsync(new WelcomeRequest
            {
                Subject = emailSubject,
                To = user.Email,
                VerifyEmailUrl = $"{_configuration["Jwt:ValidIssuer"]}/{Endpoints.Auth}/verify-email?token={_jwtSecurityTokenHandler.WriteToken(token)}"
            }, filePath);
        }

        public async Task<object> UpdateToken(string email, string oldRefreshToken, ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != oldRefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new BadRequestException("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(claimsPrincipal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            };
        }

        public async Task<bool> RevokeToken(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> VerifyEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> IsVerify(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return user.EmailConfirmed;
        }
    }
}
