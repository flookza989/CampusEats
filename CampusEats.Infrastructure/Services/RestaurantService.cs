using CampusEats.Core.Entities;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestaurantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsRestaurantActiveAsync(int restaurantId)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
                if (restaurant == null)
                    throw new RestaurantNotFoundException(restaurantId);

                return restaurant.IsActive;
            }
            catch (Exception ex) when (!(ex is RestaurantNotFoundException))
            {
                throw new DataAccessException("Get", "Restaurant", ex);
            }
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            try
            {
                return await _unitOfWork.Restaurants
                    .GetAllWithDetailsAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("GetAll", "Restaurant", ex);
            }
        }

        public async Task<Restaurant> GetRestaurantByOwnerIdAsync(int ownerId)
        {
            return await _unitOfWork.Restaurants.GetByOwnerIdAsync(ownerId);

        }

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
        {
            restaurant.CreatedAt = DateTime.UtcNow;
            restaurant.IsActive = true;

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();

            return restaurant;
        }

        public async Task UpdateRestaurantAsync(Restaurant restaurant)
        {
            var existingRestaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurant.RestaurantId);
            if (existingRestaurant == null)
                throw new RestaurantNotFoundException(restaurant.RestaurantId);

            existingRestaurant.Name = restaurant.Name;
            existingRestaurant.Description = restaurant.Description;
            existingRestaurant.Location = restaurant.Location;
            existingRestaurant.Phone = restaurant.Phone;
            existingRestaurant.OpeningTime = restaurant.OpeningTime;
            existingRestaurant.ClosingTime = restaurant.ClosingTime;
            existingRestaurant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Restaurants.UpdateAsync(existingRestaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Restaurant> GetRestaurantDetailsAsync(int restaurantId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new RestaurantNotFoundException(restaurantId);

            return restaurant;
        }

        public async Task<Restaurant> GetRestaurantDetailsWithOwnerAsync(int restaurantId)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants
                    .GetRestaurantWithDetailsAsync(restaurantId);

                if (restaurant == null)
                    throw new RestaurantNotFoundException(restaurantId);

                return restaurant;
            }
            catch (Exception ex) when (!(ex is RestaurantNotFoundException))
            {
                throw new DataAccessException("Get", "Restaurant", ex);
            }
        }

        public async Task<Restaurant> GetRestaurantForOwnerAsync(int restaurantId, int ownerId)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants
                    .GetRestaurantForOwnerAsync(restaurantId, ownerId);

                if (restaurant == null)
                    throw new RestaurantNotFoundException(restaurantId);

                return restaurant;
            }
            catch (Exception ex) when (!(ex is RestaurantNotFoundException))
            {
                throw new DataAccessException("Get", "Restaurant", ex);
            }
        }

        public async Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync()
        {
            return await _unitOfWork.Restaurants.GetActiveRestaurantsAsync();
        }

        public async Task UpdateRestaurantStatusAsync(int restaurantId, bool isActive)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new RestaurantNotFoundException(restaurantId);

            restaurant.IsActive = isActive;
            restaurant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Restaurants.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetRestaurantMenuAsync(int restaurantId)
        {
            if (!await RestaurantExistsAsync(restaurantId))
                throw new RestaurantNotFoundException(restaurantId);

            return await _unitOfWork.Restaurants.GetRestaurantMenuItemsAsync(restaurantId);
        }

        public async Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId, DateTime? date = null)
        {
            if (!await RestaurantExistsAsync(restaurantId))
                throw new RestaurantNotFoundException(restaurantId);

            if (!date.HasValue)
                return await _unitOfWork.Restaurants.GetRestaurantOrdersAsync(restaurantId);

            var startDate = date.Value.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            return await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
        }

        public async Task UpdateOperatingHoursAsync(int restaurantId, TimeSpan openingTime, TimeSpan closingTime)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new RestaurantNotFoundException(restaurantId);

            restaurant.OpeningTime = openingTime;
            restaurant.ClosingTime = closingTime;
            restaurant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Restaurants.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsRestaurantOpenAsync(int restaurantId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new RestaurantNotFoundException(restaurantId);

            if (!restaurant.IsActive)
                return false;

            var currentTime = DateTime.Now.TimeOfDay;
            return currentTime >= restaurant.OpeningTime && currentTime <= restaurant.ClosingTime;
        }

        private async Task<bool> RestaurantExistsAsync(int restaurantId)
        {
            return await _unitOfWork.Restaurants.GetByIdAsync(restaurantId) != null;
        }
    }
}
