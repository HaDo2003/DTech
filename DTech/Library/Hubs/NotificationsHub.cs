using Microsoft.AspNetCore.SignalR;

namespace DTech.Library.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task SendNewProduct(object product)
        {
            await Clients.All.SendAsync("ReceiveNewProduct", product);
        }
    }
}
