using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<UserDetailDto> Register(RegisterUserDto newUser);

        Task Login(LoginUserDto loginUser);

        JwtSecurityToken GetToken(List<Claim> authClaims, string secretKey);
    }
}
