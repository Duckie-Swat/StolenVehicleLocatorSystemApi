using StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class CameraDto : BaseDto
    {
        public Guid UserId { get; set; }

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }

        public UserDetailDto User { get; set; }

        public IEnumerable<CameraDetectedResultDto>? CameraDetectedResults { get; set; }
    }
}
