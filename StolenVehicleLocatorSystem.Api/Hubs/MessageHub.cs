using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StolenVehicleLocatorSystem.Api.Hubs.Payloads;

namespace StolenVehicleLocatorSystem.Api.Hubs
{
    
    [Authorize]
    public class MessageHub : Hub
    {
        public async Task Send(MessagePayload message)
        {
            await Clients.User(message.To!).SendAsync("SendMessage", message);
        }
    }
}
