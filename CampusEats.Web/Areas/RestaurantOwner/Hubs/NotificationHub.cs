using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace CampusEats.Web.Areas.RestaurantOwner.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinRestaurantGroup(string restaurantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Restaurant_{restaurantId}");
        }

        public async Task JoinOwnerDashboard(string restaurantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RestaurantDashboard_{restaurantId}");
        }

        public async Task UpdateOrderStatus(string orderId, string status)
        {
            await Clients.Group($"Order_{orderId}").SendAsync("OrderStatusUpdated", status);
        }

        public async Task NewOrderReceived(string restaurantId, object orderDetails)
        {
            await Clients.Group($"Restaurant_{restaurantId}").SendAsync("NewOrder", orderDetails);
        }
    }
}
