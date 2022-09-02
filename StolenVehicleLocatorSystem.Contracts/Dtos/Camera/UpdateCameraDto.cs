using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Camera
{
    public class UpdateCameraDto
    {
        public Guid Id { get; set; }
        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string? Name { get; set; }
        public Guid UserId { get; set; }
    }
}
