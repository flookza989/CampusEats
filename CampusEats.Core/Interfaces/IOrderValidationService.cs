using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IOrderValidationService
    {
        Task<bool> ValidateOrderAsync(int restaurantId, List<(int ItemId, int Quantity)> orderItems);
        Task<bool> ValidateRestaurantOperatingHoursAsync(int restaurantId);
        Task<bool> ValidateMenuItemsAvailabilityAsync(List<int> itemIds);
        Task<bool> ValidateOrderStatusTransitionAsync(int orderId, OrderStatus newStatus);
    }
}
