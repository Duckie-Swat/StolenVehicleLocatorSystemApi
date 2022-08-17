using StolenVehicleLocatorSystem.Contracts.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static StolenVehicleLocatorSystem.Contracts.Constants.ValidationRules;

namespace StolenVehicleLocatorSystem.Contracts.Filters
{
    public class BaseFilter
    {
        [Required]
        [DefaultValue(DefaultFilterCriteria.Page)]
        public int Page { get; set; } = DefaultFilterCriteria.Page;

        [Required]
        [DefaultValue(DefaultFilterCriteria.Limit)]
        public int Limit { get; set; } = DefaultFilterCriteria.Limit;

        [MaxLength(CommonRules.MaxLenghCharactersForText)]
        public string Keyword { get; set; } = "";

        public string OrderProperty { get; set; } = "";

        public bool? Desc { get; set; }

    }
}
