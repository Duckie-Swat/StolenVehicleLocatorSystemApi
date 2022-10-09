using StolenVehicleLocatorSystem.Contracts.Dtos.Camera;
using StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult
{
    public class CameraDetectedResultDto : BaseDto
    {
        public Guid CameraId { get; set; }

        public Guid LostVehicleRequestId { get; set; }

        [Range(minimum: LatitudeRule.MinValue, maximum: LatitudeRule.MaxValue)]
        public double Latitude { get; set; }

        [Range(minimum: LongtitudeRule.MinValue, maximum: LongtitudeRule.MaxValue)]
        public double Longitude { get; set; }

        public string Location { get; set; }

        public string Photo { get; set; }

        public string PlateNumber { get; set; }

        public CameraDto Camera { get; set; }
        public LostVehicleRequestDto LostVehicleRequest { get; set; }
    }
}
