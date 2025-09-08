using Microsoft.AspNetCore.SignalR;

namespace RMS.Hubs
{
    public class KdsHub : Hub
    {
        public async Task NotifyOrderReady(int orderId)
        {
            await Clients.All.SendAsync("OrderReady", orderId);
        }

        public async Task NewOrder(int orderId)
        {
            await Clients.All.SendAsync("NewOrder", orderId);
        }
    }
}
