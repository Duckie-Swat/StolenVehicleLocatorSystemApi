using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class UpdateCameraDto
    {
        public Guid Id { get; set; }
        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }
        public Guid UserId { get; set; }
    }
}
