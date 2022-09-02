using StolenVehicleLocatorSystem.Contracts.Constants;
using System.ComponentModel;

namespace StolenVehicleLocatorSystem.Contracts.Filters
{
    public class UserSearch : BaseSearch
    {
        [DefaultValue(DefaultFilterCriteria.IsDeleted)]
        public bool? IsDeleted { get; set; }
    }
}
