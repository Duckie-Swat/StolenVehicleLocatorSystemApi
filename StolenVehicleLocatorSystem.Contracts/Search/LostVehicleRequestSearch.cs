using System.ComponentModel;
using StolenVehicleLocatorSystem.DataAccessor.Constants;

namespace StolenVehicleLocatorSystem.Contracts.Filters
{
    public class LostVehicleRequestSearch : BaseSearch
    {
        [DefaultValue(LostVehicleRequestStatus.PROCESSING)]
        public LostVehicleRequestStatus? Status { get; set; }
    }
}
