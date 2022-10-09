using StolenVehicleLocatorSystem.Contracts.Constants;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult
{
    public class CreateCameraDetectedResultDto
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
    }
}
