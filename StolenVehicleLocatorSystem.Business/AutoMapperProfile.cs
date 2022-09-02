using StolenVehicleLocatorSystem.Contracts.Dtos.Auth;
using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
using StolenVehicleLocatorSystem.Contracts.Dtos.Notification;
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
            CreateMap<UserTokenDto, UserToken>();

            // User 
            CreateMap<UserDetailDto, User>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<LoginUserDto, User>();

            // Notification
            CreateMap<NotificationDto, Notification>();
            CreateMap<CreateNotificationDto, Notification>();

            // Camera
            CreateMap<CameraDto, Camera>();
            CreateMap<CreateCameraDto, Camera>();
        }

        private void FromDataAccessorLayer()
        {
            CreateMap<User, UserDetailDto>();
            CreateMap<UserToken, UserTokenDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<Camera, CameraDto>();
        }
    }
}
