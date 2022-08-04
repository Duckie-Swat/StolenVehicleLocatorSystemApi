using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace StolenVehicleLocatorSystem.DataAccessor.Persistence
{
    public class MongoDbPersistence
    {
        public static void Configure()
        {
            UserMap.Configure();

            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // Conventions
            var pack = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true),
                    new IgnoreIfDefaultConvention(true)
                };
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);
        }
    }
}
