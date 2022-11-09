using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Constants;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.Contracts.Models;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace StolenVehicleLocatorSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMailKitEmailService _mailKitEmailService;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUserTokenService _userTokenService;
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(UserManager<User> userManager,
            RoleManager<Role> roleManager, IMapper mapper,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IUserTokenService userTokenService,
            IMailKitEmailService mailKitEmailService,
            ITokenService tokenService,
            IHttpClientFactory httpClientFactory
            )
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _userTokenService = userTokenService;
            _mailKitEmailService = mailKitEmailService;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<bool> VerifyGoogleRecaptcha(VerifyCaptchaModel verifyCaptchaModel)
        {
            string requestUri = $"https://www.google.com/recaptcha/api/siteverify?secret={verifyCaptchaModel.Secret}&response={verifyCaptchaModel.Response}";
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync(requestUri, null);
            string result = await httpResponseMessage.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(result))
            {
                JsonElement root = doc.RootElement;
                var success = root.GetProperty("success").ToString();
                return success == "True";
            }
        }


        public async Task<TokenResponse> Login(LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (user.IsDeleted)
                    throw new HttpStatusException(HttpStatusCode.Forbidden, "This user deleted. Please contact admin to solve this problem.");
                
                if (!(await VerifyGoogleRecaptcha(new VerifyCaptchaModel
                {
                    Response = loginUser.ResponseCaptchaToken,
                    Secret = _configuration["ExternalProviders:Google:RecaptchaV2SecretKey"]
                })) && !roles.Any(x => x == "Admin")
                )
                {
                    throw new BadRequestException("Recaptcha code is not valid or expire");
                }
                if (await _userManager.CheckPasswordAsync(user, loginUser.Password))
                {
                    var authClaims = await _userManager.GetClaimsAsync(user);
                    _ = double.TryParse(_configuration["JWT:TokenValidityInMinutes"], out double tokenValidityInMinutes);
                    var token = _tokenService.CreateAccessToken(authClaims, tokenValidityInMinutes);
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
                            user.Id,
                            DisplayName = user.FirstName + " " + user.LastName,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            user.PhoneNumber,
                            user.DateOfBirth
                        }
                    };
                }
            }

            throw new BadRequestException("username or password is not correct");
        }

        public async Task<TokenResponse> Register(RegisterUserDto newUser)
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

            if (!(await VerifyGoogleRecaptcha(new VerifyCaptchaModel
            {
                Response = newUser.ResponseCaptchaToken,
                Secret = _configuration["ExternalProviders:Google:RecaptchaV2SecretKey"]
            })))
            {
                throw new BadRequestException("Recaptcha code is not valid or expire");
            }

            var user = _mapper.Map<User>(newUser);
            user.UserName = user.Email;
            var newUserResult = await _userManager.CreateAsync(user, newUser.Password);

            if (newUserResult.Errors.Any())
            {
                throw new BadRequestException(string.Join(',', newUserResult.Errors.Select(e => e.Description)));
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

            return await Login(new LoginUserDto
            {
                Email = newUser.Email,
                Password = newUser.Password
            });
        }

        public async Task SendVerifyEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var authClaims = await _userManager.GetClaimsAsync(user);

            var token = _tokenService.CreateAccessToken(authClaims, 60);

            var emailSubject = "Verify Your Email";
            string filePath = Path.Combine(new string[]
            {
                Directory.GetCurrentDirectory(),
                "Templates",
                "WelcomeTemplate.html"
            });

            await _mailKitEmailService.SendWelcomeEmailAsync(new WelcomeResponse
            {
                Subject = emailSubject,
                To = user.Email,
                VerifyEmailUrl = $"{_configuration["Jwt:ValidIssuer"]}/{Endpoints.Auth}/verify-email?token={_jwtSecurityTokenHandler.WriteToken(token)}"
            }, filePath);
        }

        private string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
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
            if (user == null)
                throw new BadRequestException("User doesn't exist");
            if (await _userManager.CheckPasswordAsync(user, oldPassword))
            {
                if (oldPassword == newPassword)
                    throw new BadRequestException("New password shouldn't be the same with old password");
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (result.Errors.Any())
                {
                    throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
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

        public async Task SendResetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var emailSubject = "Reset your password";
            string filePath = Path.Combine(new string[]
            {
                Directory.GetCurrentDirectory(),
                "Templates",
                "ResetPasswordTemplate.html"
            });
            string randomPassword = GeneratePassword();

            await _mailKitEmailService.SendResetPasswordEmailAsync(new ResetEmailResponse
            {
                Subject = emailSubject,
                To = user.Email,
                ResetPasswordUrl = $"{_configuration["Jwt:ValidIssuer"]}/{Endpoints.Auth}/reset-password/accept?token={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))}&email={user.Email}&password={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(randomPassword))}",
                NewPassword = randomPassword
            }, filePath);
        }

        public async Task ResetPassword(string token, string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException("User doesn't exist");
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            password = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(password));
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Errors.Any())
            {
                throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
