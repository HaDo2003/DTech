using Microsoft.AspNetCore.SignalR;

namespace DTech.Library.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task SendProductUpdate(object product)
        {
            await Clients.All.SendAsync("ReceiveProductUpdate", product);
        }
    }
}
