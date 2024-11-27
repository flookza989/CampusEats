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
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuItem> AddMenuItemAsync(MenuItem menuItem)
        {
            if (!await RestaurantExistsAsync(menuItem.RestaurantId))
                throw new RestaurantNotFoundException(menuItem.RestaurantId);

            menuItem.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.MenuItems.AddAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();

            return menuItem;
        }

        public async Task UpdateMenuItemAsync(MenuItem menuItem)
        {
            var existingItem = await _unitOfWork.MenuItems.GetByIdAsync(menuItem.ItemId);
            if (existingItem == null)
                throw new MenuItemNotFoundException(menuItem.ItemId);

            existingItem.Name = menuItem.Name;
            existingItem.Description = menuItem.Description;
            existingItem.Price = menuItem.Price;
            existingItem.ImageUrl = menuItem.ImageUrl;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.MenuItems.UpdateAsync(existingItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<MenuItem> GetMenuItemDetailsAsync(int itemId)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
            if (menuItem == null)
                throw new MenuItemNotFoundException(itemId);

            return menuItem;
        }

        public async Task UpdateMenuItemAvailabilityAsync(int itemId, bool isAvailable)
        {
            await ValidateMenuItemExistsAsync(itemId);
            await _unitOfWork.MenuItems.UpdateAvailabilityAsync(itemId, isAvailable);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm, int? restaurantId = null)
        {
            var items = await _unitOfWork.MenuItems.SearchMenuItemsAsync(searchTerm);

            if (restaurantId.HasValue)
                items = items.Where(i => i.RestaurantId == restaurantId.Value);

            return items;
        }

        public async Task UpdateMenuItemPriceAsync(int itemId, decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ValidationException(nameof(newPrice), newPrice, "Price must be greater than zero.");

            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
            if (menuItem == null)
                throw new MenuItemNotFoundException(itemId);

            menuItem.Price = newPrice;
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.MenuItems.UpdateAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int restaurantId)
        {
            if (!await RestaurantExistsAsync(restaurantId))
                throw new RestaurantNotFoundException(restaurantId);

            return await _unitOfWork.MenuItems.GetAllMenuItemsAsync(restaurantId);
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync(int restaurantId)
        {
            if (!await RestaurantExistsAsync(restaurantId))
                throw new RestaurantNotFoundException(restaurantId);

            return await _unitOfWork.MenuItems.GetAvailableMenuItemsAsync(restaurantId);
        }

        public async Task DeleteMenuItemAsync(int itemId)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
            if (menuItem == null)
                throw new MenuItemNotFoundException(itemId);

            // Check if the menu item has any associated orders
            var orderItems = await _unitOfWork.OrderItems.GetOrderItemsByMenuItemAsync(itemId);
            if (orderItems.Any())
            {
                // Instead of deleting, mark as unavailable
                menuItem.IsAvailable = false;
                menuItem.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.MenuItems.UpdateAsync(menuItem);
            }
            else
            {
                await _unitOfWork.MenuItems.DeleteAsync(itemId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> RestaurantExistsAsync(int restaurantId)
        {
            return await _unitOfWork.Restaurants.GetByIdAsync(restaurantId) != null;
        }

        private async Task ValidateMenuItemExistsAsync(int itemId)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);
            if (menuItem == null)
                throw new MenuItemNotFoundException(itemId);
        }
    }
}
