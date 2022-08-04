using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using StolenVehicleLocatorSystem.DataAccessor.Constants;

namespace StolenVehicleLocatorSystem.DataAccessor.Models
{
    [CollectionName(CollectionName.Users)]
    public class User : MongoIdentityUser<Guid>
    {

    }
}
