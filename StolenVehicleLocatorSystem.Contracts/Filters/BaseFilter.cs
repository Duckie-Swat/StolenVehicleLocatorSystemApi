using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Filters
{
    public class BaseFilter
    {
        [Required]
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [Required]
        [DefaultValue(20)]
        public int Limit { get; set; } = 20;

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string Keyword { get; set; } = "";

        public string OrderProperty { get; set; } = "";

        public bool? Desc { get; set; }

    }
}
