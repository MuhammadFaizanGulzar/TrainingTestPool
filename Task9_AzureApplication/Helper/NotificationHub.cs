using Microsoft.AspNetCore.SignalR;

namespace Task9_AzureApplication.Helper
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            //A connection with the hub is established.

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // When a connection is terminated.

            await base.OnDisconnectedAsync(exception);
        }
    }
}
