using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Enums;
using System.ComponentModel.DataAnnotations;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Notification
{
    public class NotificationDto : BaseDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        [MinLength(Constants.ValidationRules.CommonRules.MinLenghCharactersForText)]
        public string Title { get; set; }

        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string Description { get; set; }
        public NotificationTypeEnum Type { get; set; }

        public bool IsUnRead { get; set; } = true;

        public Guid UserId { get; set; }
        public UserDetailDto User { get; set; }
    }
}
