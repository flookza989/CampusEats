using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IRestaurantRepository : IRepository<Restaurant>
    {
        Task<IEnumerable<Restaurant>> GetAllWithDetailsAsync();
        Task<Restaurant> GetByOwnerIdAsync(int ownerId);
        Task<Restaurant> GetRestaurantWithDetailsAsync(int restaurantId);
        Task<Restaurant> GetRestaurantForOwnerAsync(int restaurantId, int ownerId);
        Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync();
        Task<IEnumerable<MenuItem>> GetRestaurantMenuItemsAsync(int restaurantId);
        Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId);
        Task<IEnumerable<Order>> GetRestaurantOrdersByStatusAsync(int restaurantId, OrderStatus status);
    }
}
