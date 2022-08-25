using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Api.Hubs.Providers
{
    public class EmailBasedUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
        }
    }
}
