using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Order> CreateOrderAsync(int userId, int restaurantId, List<(int ItemId, int Quantity, string SpecialInstructions)> orderItems);
        Task<Order> GetOrderDetailsAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task CancelOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetPendingOrdersAsync(int restaurantId);
        Task<decimal> CalculateOrderTotalAsync(List<(int ItemId, int Quantity)> orderItems);
        Task<bool> CanUserCancelOrderAsync(int orderId, int userId);
        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId);
        Task<IEnumerable<Notification>> GetOrderStatusUpdatesAsync(int orderId);
    }
}
