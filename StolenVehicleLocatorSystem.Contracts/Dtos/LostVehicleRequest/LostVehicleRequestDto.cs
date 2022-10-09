using StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult;
using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Constants;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest
{
    public class LostVehicleRequestDto : BaseDto
    {
        public Guid UserId { get; set; }

        [MinLength(PlateNumberRule.MinLenghCharactersForText)]
        [MaxLength(PlateNumberRule.MaxLenghCharactersForText)]
        public string PlateNumber { get; set; }

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string VehicleType { get; set; }

        [Range(minimum: LatitudeRule.MinValue, maximum: LatitudeRule.MaxValue)]
        public double Latitude { get; set; }

        [Range(minimum: LongtitudeRule.MinValue, maximum: LongtitudeRule.MaxValue)]
        public double Longitude { get; set; }

        public string Location { get; set; }

        public LostVehicleRequestStatus Status { get; set; }

        public UserDetailDto User { get; set; }

        public IEnumerable<CameraDetectedResultDto> CameraDetectedResults { get; set; }
    }
}
