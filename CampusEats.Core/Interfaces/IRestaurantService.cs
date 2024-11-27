using CampusEats.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IRestaurantService
    {
        Task<Restaurant> GetRestaurantDetailsWithOwnerAsync(int restaurantId);
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
        Task<bool> IsRestaurantActiveAsync(int restaurantId);
        Task<Restaurant> GetRestaurantByOwnerIdAsync(int ownerId);
        Task<Restaurant> GetRestaurantForOwnerAsync(int restaurantId, int ownerId);
        Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
        Task UpdateRestaurantAsync(Restaurant restaurant);
        Task<Restaurant> GetRestaurantDetailsAsync(int restaurantId);
        Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync();
        Task UpdateRestaurantStatusAsync(int restaurantId, bool isActive);
        Task<IEnumerable<MenuItem>> GetRestaurantMenuAsync(int restaurantId);
        Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId, DateTime? date = null);
        Task UpdateOperatingHoursAsync(int restaurantId, TimeSpan openingTime, TimeSpan closingTime);
        Task<bool> IsRestaurantOpenAsync(int restaurantId);
    }
}
