using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Auth
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [DefaultValue("admin@duckieswat.com")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        [DefaultValue("Str0ng!Passw0rd")]
        public string Password { get; set; }

        public string ResponseCaptchaToken { get; set; }
    }
}
