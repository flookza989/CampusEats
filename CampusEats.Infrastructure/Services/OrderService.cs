using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderValidationService _validationService;
        private readonly INotificationService _notificationService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IOrderValidationService validationService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // ตรวจสอบว่าร้านอาหารมีอยู่จริง
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new RestaurantNotFoundException(restaurantId);

            // ถ้าไม่ระบุวันที่ ให้ดึงออเดอร์ทั้งหมด
            if (!startDate.HasValue && !endDate.HasValue)
            {
                return await _unitOfWork.Orders.GetOrdersByRestaurantAsync(restaurantId);
            }

            // ถ้าระบุแค่ startDate ให้ดึงจนถึงวันปัจจุบัน
            if (startDate.HasValue && !endDate.HasValue)
            {
                endDate = DateTime.UtcNow;
            }

            // ถ้าระบุแค่ endDate ให้ดึงย้อนหลัง 30 วัน
            if (!startDate.HasValue && endDate.HasValue)
            {
                startDate = endDate.Value.AddDays(-30);
            }

            return await _unitOfWork.Orders.GetOrdersByDateRangeAsync(
                restaurantId,
                startDate.Value,
                endDate.Value);
        }

        public async Task<Order> CreateOrderAsync(int userId, int restaurantId, List<(int ItemId, int Quantity, string SpecialInstructions)> orderItems)
        {
            // Validate restaurant is open
            if (!await _validationService.ValidateRestaurantOperatingHoursAsync(restaurantId))
                throw new RestaurantClosedException(restaurantId, TimeSpan.Zero, TimeSpan.Zero); // Times will be filled by the validation service

            // Validate menu items are available
            var itemIds = orderItems.Select(x => x.ItemId).ToList();
            if (!await _validationService.ValidateMenuItemsAvailabilityAsync(itemIds))
                throw new MenuItemNotAvailableException(0, "One or more items are not available");

            var order = new Order
            {
                UserId = userId,
                RestaurantId = restaurantId,
                OrderTime = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Create order items
            var orderItemEntities = new List<OrderItem>();
            foreach (var item in orderItems)
            {
                var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(item.ItemId);
                if (menuItem == null)
                    throw new MenuItemNotFoundException(item.ItemId);

                orderItemEntities.Add(new OrderItem
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    UnitPrice = menuItem.Price,
                    SpecialInstructions = item.SpecialInstructions,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Calculate total
            order.TotalAmount = orderItemEntities.Sum(oi => oi.UnitPrice * oi.Quantity);

            // Save order
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Add order items
            foreach (var orderItem in orderItemEntities)
            {
                orderItem.OrderId = order.OrderId;
                await _unitOfWork.OrderItems.AddAsync(orderItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Create initial notification
            await _notificationService.CreateOrderStatusNotificationAsync(order.OrderId, OrderStatus.Pending);

            return order;
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            return order;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            if (!await _validationService.ValidateOrderStatusTransitionAsync(orderId, newStatus))
                throw new InvalidOrderStatusTransitionException(order.Status, newStatus);

            await _unitOfWork.Orders.UpdateOrderStatusAsync(orderId, newStatus);
            await _unitOfWork.SaveChangesAsync();

            // Create notification for status change
            await _notificationService.CreateOrderStatusNotificationAsync(orderId, newStatus);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            if (order.Status != OrderStatus.Pending)
                throw new OrderCancellationException(orderId, order.Status);

            await UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync(int restaurantId)
        {
            return await _unitOfWork.Orders.GetOrdersByStatusAsync(OrderStatus.Pending);
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<(int ItemId, int Quantity)> orderItems)
        {
            decimal total = 0;
            foreach (var item in orderItems)
            {
                var price = await _unitOfWork.MenuItems.GetMenuItemPriceAsync(item.ItemId);
                total += price * item.Quantity;
            }
            return total;
        }

        public async Task<bool> CanUserCancelOrderAsync(int orderId, int userId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return false;

            return order.UserId == userId && order.Status == OrderStatus.Pending;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            if (!await OrderExistsAsync(orderId))
                throw new OrderNotFoundException(orderId);

            return await _unitOfWork.Orders.GetOrderItemsAsync(orderId);
        }

        private async Task<bool> OrderExistsAsync(int orderId)
        {
            return await _unitOfWork.Orders.GetByIdAsync(orderId) != null;
        }

        public async Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId)
        {
            return await _unitOfWork.Orders.GetOrdersByUserAsync(userId);
        }

        public async Task<IEnumerable<Notification>> GetOrderStatusUpdatesAsync(int orderId)
        {
            return await _unitOfWork.Notifications.GetNotificationsByOrderAsync(orderId);
        }
    }
}
