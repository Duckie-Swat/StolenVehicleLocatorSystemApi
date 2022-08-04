using MongoDB.Bson.Serialization;
using StolenVehicleLocatorSystem.DataAccessor.Models;

namespace StolenVehicleLocatorSystem.DataAccessor.Persistence
{
    public class UserMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
    }
}
