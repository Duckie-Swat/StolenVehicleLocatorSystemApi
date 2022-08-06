using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
