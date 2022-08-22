using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class ChangePasswordDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string NewPassword { get; set; }

    }
}
