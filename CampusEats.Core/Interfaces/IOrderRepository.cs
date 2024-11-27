using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByRestaurantAsync(int restaurantId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(int restaurantId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId);
        Task<decimal> CalculateOrderTotalAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
    }
}
