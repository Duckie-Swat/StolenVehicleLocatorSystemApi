using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class CameraDto : BaseDto
    {
        public Guid UserId { get; set; }

        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }

        public UserDetailDto User { get; set; }
    }
}
