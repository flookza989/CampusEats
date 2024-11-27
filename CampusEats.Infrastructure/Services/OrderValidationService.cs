using CampusEats.Core.Enums;
using CampusEats.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ValidateOrderAsync(int restaurantId, List<(int ItemId, int Quantity)> orderItems)
        {
            // Check if restaurant exists and is active
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null || !restaurant.IsActive)
                return false;

            // Validate operating hours
            if (!await ValidateRestaurantOperatingHoursAsync(restaurantId))
                return false;

            // Validate all items exist and are available
            foreach (var (itemId, quantity) in orderItems)
            {
                var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
                if (menuItem == null || !menuItem.IsAvailable || menuItem.RestaurantId != restaurantId)
                    return false;

                if (quantity <= 0)
                    return false;
            }

            return true;
        }

        public async Task<bool> ValidateRestaurantOperatingHoursAsync(int restaurantId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                return false;

            var currentTime = DateTime.Now.TimeOfDay;
            return currentTime >= restaurant.OpeningTime &&
                   currentTime <= restaurant.ClosingTime;
        }

        public async Task<bool> ValidateMenuItemsAvailabilityAsync(List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
                if (menuItem == null || !menuItem.IsAvailable)
                    return false;
            }
            return true;
        }

        public async Task<bool> ValidateOrderStatusTransitionAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return false;

            // Define valid status transitions
            var validTransitions = new Dictionary<OrderStatus, HashSet<OrderStatus>>
            {
                {
                    OrderStatus.Pending,
                    new HashSet<OrderStatus> { OrderStatus.Preparing, OrderStatus.Cancelled }
                },
                {
                    OrderStatus.Preparing,
                    new HashSet<OrderStatus> { OrderStatus.Ready }
                },
                {
                    OrderStatus.Ready,
                    new HashSet<OrderStatus> { OrderStatus.Completed }
                },
                {
                    OrderStatus.Completed,
                    new HashSet<OrderStatus>()  // No further transitions allowed
                },
                {
                    OrderStatus.Cancelled,
                    new HashSet<OrderStatus>()  // No further transitions allowed
                }
            };

            return validTransitions.ContainsKey(order.Status) &&
                   validTransitions[order.Status].Contains(newStatus);
        }
    }
}
