using StolenVehicleLocatorSystem.Contracts.Attributes;
using StolenVehicleLocatorSystem.DataAccessor.Constants;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.LostVehicleRequest
{
    public class CreateLostVehicleRequestDto
    {
        public Guid UserId { get; set; }

        [MinLength(Constants.ValidationRules.PlateNumberRule.MinLenghCharactersForText)]
        [MaxLength(Constants.ValidationRules.PlateNumberRule.MaxLenghCharactersForText)]
        public string PlateNumber { get; set; }

        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string VehicleType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Location { get; set; }

        [SwaggerIgnore]
        public LostVehicleRequestStatus Status { get; set; } = LostVehicleRequestStatus.PROCESSING;
    }
}
