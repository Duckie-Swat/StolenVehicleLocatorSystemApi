using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IBaseRepository<UserToken> _userToken;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;


        public UserTokenService(IBaseRepository<UserToken> userToken,
            IMapper mapper, ITokenService tokenService)
        {
            _userToken = userToken;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<bool> CreateUserToken(CreateUserTokenDto createUserTokenDto)
        {
            var userToken = _mapper.Map<UserToken>(createUserTokenDto);
            try
            {
                await _userToken.AddAsync(userToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserTokenDto> GetByRefreshToken(string refreshToken)
        {
            var userToken = await _userToken.GetByAsync(p => p.RefreshToken == refreshToken);

            if (userToken == null)
            {
                throw new BadRequestException("Refresh token is invalid");
            }

            return _mapper.Map<UserTokenDto>(userToken);
        }

        public Task<bool> RevokeAllToken(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RevokeToken(string refreshToken)
        {
            var query = _userToken.Entities;
            var userToken = query.FirstOrDefault(x => x.RefreshToken == refreshToken);

            try
            {
                await _userToken.DeleteAsync(userToken.Id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<object> UpdateToken(Guid userId, string oldRefreshToken, ClaimsPrincipal claimsPrincipal)
        {

            var userToken = await _userToken.GetByAsync(p => p.RefreshToken == oldRefreshToken);

            if (userToken == null)
            {
                throw new BadRequestException("Invalid access token or refresh token");
            }

            if (userToken.RefreshTokenExpiryTime <= DateTime.Now)
            {
                await RevokeToken(oldRefreshToken);
                throw new BadRequestException("Refresh token is expired");
            }

            var newAccessToken = _tokenService.CreateAccessToken(claimsPrincipal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            userToken.RefreshToken = newRefreshToken;
            await _userToken.UpdateAsync(userToken);
            return new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            };
        }
    }
}
