using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.User
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public bool EmailConfirmed { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
