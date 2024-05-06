using Microsoft.AspNetCore.SignalR;

namespace BookManagement.Hubs
{
    public class BookHub : Hub
    {
        public async Task SendMessage(string msg)
        {
            await Clients.All.SendAsync("ReceiveMessage");
        }
    }
}
