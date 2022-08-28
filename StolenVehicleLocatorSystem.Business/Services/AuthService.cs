using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Models;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        private readonly IUserTokenService _userTokenService;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IUserTokenService userTokenService,
            IEmailSender emailSender,
            IMailKitEmailService mailKitEmailService,
            ITokenService tokenService
            )
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _userTokenService = userTokenService;
            _emailSender = emailSender;
            _mailKitEmailService = mailKitEmailService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> Login(LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var authClaims = await _userManager.GetClaimsAsync(user);
                var token = _tokenService.CreateAccessToken(authClaims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                var userToken = new CreateUserTokenDto
                {
                    RefreshToken = refreshToken,
                    Platform = "Test",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays),
                    UserId = user.Id
                };
                await _userTokenService.CreateUserToken(userToken);

                return new TokenResponse
                {

                    RefreshToken = refreshToken,
                    AccessToken = _jwtSecurityTokenHandler.WriteToken(token),
                    AccessTokenExpiration = token.ValidTo,
                    RefreshTokenExpiration = userToken.RefreshTokenExpiryTime,
                    User = new
                    {
                        Id = user.Id,
                        DisplayName = user.FirstName + " " + user.LastName
                    }
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
                throw new BadRequestException("Role is not exist");
            }

            var user = _mapper.Map<User>(newUser);
            user.UserName = user.Email;
            var newUserResult = await _userManager.CreateAsync(user, newUser.Password);

            if (newUserResult.Errors.Any())
            {
                throw new BadRequestException(string.Join(',',newUserResult.Errors.Select(e => e.Description)));
            }

            var authClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.Role, role),
                    new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                    new Claim(JwtClaimTypes.GivenName, user.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, user.LastName)
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

            var token = _tokenService.CreateAccessToken(authClaims);

            var emailSubject = "Verify Your Email";
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
            await _mailKitEmailService.SendWelcomeEmailAsync(new WelcomeRequest
            {
                Subject = emailSubject,
                To = user.Email,
                VerifyEmailUrl = $"{_configuration["Jwt:ValidIssuer"]}/{Endpoints.Auth}/verify-email?token={_jwtSecurityTokenHandler.WriteToken(token)}"
            }, filePath);
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

        public async Task ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                throw new BadRequestException("User doesn't exist");
            if(await _userManager.CheckPasswordAsync(user, oldPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if(result.Errors.Any())
                {
                    throw new BadRequestException(string.Join(",", result.Errors.Select(e => e.Description)));
                }
            }
            else
                throw new BadRequestException("Password doesn't correct");
        }

        public async Task<bool> IsRefreshTokenValid(string refreshToken, Guid userId)
        {
            var refreshTokenInfor = await _userTokenService.GetByRefreshToken(refreshToken);
            if (refreshTokenInfor.UserId == userId)
                return true;
            else
                return false;
        }
    }
}
