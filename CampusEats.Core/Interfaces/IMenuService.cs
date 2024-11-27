using CampusEats.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IMenuService
    {
        Task<MenuItem> AddMenuItemAsync(MenuItem menuItem);
        Task UpdateMenuItemAsync(MenuItem menuItem);
        Task<MenuItem> GetMenuItemDetailsAsync(int itemId);
        Task UpdateMenuItemAvailabilityAsync(int itemId, bool isAvailable);
        Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm, int? restaurantId = null);
        Task UpdateMenuItemPriceAsync(int itemId, decimal newPrice);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync(int restaurantId);
        Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int restaurantId);
        Task DeleteMenuItemAsync(int itemId);
    }
}
