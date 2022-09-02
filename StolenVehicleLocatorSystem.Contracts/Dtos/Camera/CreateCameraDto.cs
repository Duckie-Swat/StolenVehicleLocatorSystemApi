using StolenVehicleLocatorSystem.Contracts.Attributes;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class CreateCameraDto
    {
        [SwaggerIgnore]
        public Guid UserId { get; set; }

        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }
    }
}
