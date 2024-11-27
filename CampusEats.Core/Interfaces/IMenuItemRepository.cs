using CampusEats.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int restaurantId);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync(int restaurantId);
        Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm);
        Task UpdateAvailabilityAsync(int itemId, bool isAvailable);
        Task<decimal> GetMenuItemPriceAsync(int itemId);
    }
}
