using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StolenVehicleLocatorSystem.Api.Hubs.Payloads;
using StolenVehicleLocatorSystem.Business.Interfaces;
using StolenVehicleLocatorSystem.DataAccessor.Entities;

namespace StolenVehicleLocatorSystem.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationSerivce _notificationSerivce;
        private readonly UserManager<User> _userManager;

        public NotificationHub(INotificationSerivce notificationSerivce, UserManager<User> userManager)
        {
            _notificationSerivce = notificationSerivce;
            _userManager = userManager;
        }

        public async Task Send(NotificationPayload message)
        {
            var user = await _userManager.FindByEmailAsync(message.To);
            message.Content.UserId = user.Id;
            var notification =  await _notificationSerivce.CreateAsync(message.Content);
            await Clients.User(message.To!).SendAsync("SendNotification", notification);
        }
    }
}
