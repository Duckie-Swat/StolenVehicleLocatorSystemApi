using StolenVehicleLocatorSystem.DataAccessor.Interfaces;
using StolenVehicleLocatorSystem.DataAccessor.Models;

namespace StolenVehicleLocatorSystem.DataAccessor.Repository
{

    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoContext context) : base(context)
        {
        }
    }
}
