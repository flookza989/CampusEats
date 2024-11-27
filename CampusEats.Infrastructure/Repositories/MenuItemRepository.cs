using CampusEats.Core.Entities;
using CampusEats.Core.Interfaces;
using CampusEats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync(int restaurantId)
        {
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync(int restaurantId)
        {
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId && m.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm)
        {
            return await _context.MenuItems
                .Include(m => m.Restaurant)
                .Where(m => m.Name.Contains(searchTerm) ||
                           m.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task UpdateAvailabilityAsync(int itemId, bool isAvailable)
        {
            var menuItem = await GetByIdAsync(itemId);
            if (menuItem != null)
            {
                menuItem.IsAvailable = isAvailable;
                await UpdateAsync(menuItem);
            }
        }

        public async Task<decimal> GetMenuItemPriceAsync(int itemId)
        {
            var menuItem = await GetByIdAsync(itemId);
            return menuItem?.Price ?? 0;
        }
    }
}
