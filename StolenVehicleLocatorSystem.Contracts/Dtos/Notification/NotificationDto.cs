using StolenVehicleLocatorSystem.Contracts.Dtos.User;
using StolenVehicleLocatorSystem.DataAccessor.Enums;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Notification
{
    public class NotificationDto : BaseDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        [MinLength(CommonRules.MinLenghCharactersForText)]
        public string Title { get; set; }

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string Description { get; set; }
        public NotificationTypeEnum Type { get; set; }

        public bool IsUnRead { get; set; } = true;

        public Guid UserId { get; set; }
        public UserDetailDto User { get; set; }
    }
}
