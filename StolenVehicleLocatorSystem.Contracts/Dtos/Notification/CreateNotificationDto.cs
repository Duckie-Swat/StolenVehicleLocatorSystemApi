using StolenVehicleLocatorSystem.DataAccessor.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StolenVehicleLocatorSystem.Contracts.Dtos.Notification
{
    public class CreateNotificationDto
    {
        [Required]
        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        [MinLength(Constants.ValidationRules.CommonRules.MinLenghCharactersForText)]
        public string Title { get; set; }

        [MaxLength(Constants.ValidationRules.CommonRules.MaxLenghCharactersForText)]
        public string Description { get; set; }
        public NotificationTypeEnum Type { get; set; }

        public Guid UserId { get; set; }
    }
}
