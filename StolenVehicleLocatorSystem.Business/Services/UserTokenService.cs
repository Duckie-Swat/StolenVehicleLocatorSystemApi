using AutoMapper;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.DataAccessor.Entities;
using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.DataAccessor.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StolenVehicleLocatorSystem.Business.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IBaseRepository<UserToken> _userToken;
        private readonly IMapper _mapper;

        public UserTokenService(IBaseRepository<UserToken> userToken, IMapper mapper)
        {
            _userToken = userToken;
            _mapper = mapper;
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

        public Task<bool> UpdateUserToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
