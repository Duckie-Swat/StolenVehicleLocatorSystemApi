using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using StolenVehicleLocatorSystem.DataAccessor.Constants;

namespace StolenVehicleLocatorSystem.DataAccessor.Models
{
    [CollectionName(CollectionName.Users)]
    public class User : MongoIdentityUser<Guid>
    {
        public DateTime UpdatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public Guid? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
