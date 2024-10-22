﻿using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Models;

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
