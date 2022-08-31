using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.User
{
    public class UpdateUserDto
    {
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
    }
}
