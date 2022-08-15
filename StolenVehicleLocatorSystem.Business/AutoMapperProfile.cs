using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Entities;

namespace StolenVehicleLocatorSystem.Business
{
    public class AutoMapperProfile : AutoMapper.Profile
    {

        public AutoMapperProfile()
        {
            FromDataAccessorLayer();
            FromPresentationLayer();
        }

        private void FromPresentationLayer()
        {
            //User Token
            CreateMap<CreateUserTokenDto, UserToken>();

            // User 
            CreateMap<UserDetailDto, User>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<LoginUserDto, User>();
        }

        private void FromDataAccessorLayer()
        {
            CreateMap<User, UserDetailDto>();
        }
    }
}
