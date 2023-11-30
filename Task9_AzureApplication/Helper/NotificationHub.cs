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
            // This method is called when a connection with the hub is established.

            // You can use it to track connections or perform other tasks.
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // This method is called when a connection is terminated.

            // You can use it to track disconnections or perform other cleanup tasks.
            await base.OnDisconnectedAsync(exception);
        }
    }
}
