using Microsoft.AspNetCore.SignalR;

namespace SmartNotificationService
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.User(user).SendAsync("RecieveNotification", message);
        }
    }
}
