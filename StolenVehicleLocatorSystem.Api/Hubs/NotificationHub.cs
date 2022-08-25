using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StolenVehicleLocatorSystem.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {

        }

    }
}
