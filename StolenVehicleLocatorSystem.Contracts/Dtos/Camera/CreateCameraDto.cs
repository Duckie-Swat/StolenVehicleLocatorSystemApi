using StolenVehicleLocatorSystem.Contracts.Attributes;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class CreateCameraDto
    {
        [SwaggerIgnore]
        public Guid UserId { get; set; }

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }
    }
}
