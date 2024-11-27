using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
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
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Restaurant>> GetAllWithDetailsAsync()
        {
            return await _context.Restaurants
                .Include(r => r.Owner)
                .Include(r => r.MenuItems)
                .Include(r => r.Orders)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Restaurant> GetByOwnerIdAsync(int ownerId)
        {
            return await _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.OwnerId == ownerId);
        }
        public async Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync()
        {
            return await _context.Restaurants
                .Where(r => r.IsActive)
                .Include(r => r.MenuItems.Where(m => m.IsAvailable))
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetRestaurantMenuItemsAsync(int restaurantId)
        {
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRestaurantOrdersAsync(int restaurantId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == restaurantId)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRestaurantOrdersByStatusAsync(int restaurantId, OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == restaurantId && o.Status == status)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();
        }

        public async Task<Restaurant> GetRestaurantWithDetailsAsync(int restaurantId)
        {
            return await _context.Restaurants
                .Include(r => r.Owner)
                .Include(r => r.MenuItems)
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        }

        public async Task<Restaurant> GetRestaurantForOwnerAsync(int restaurantId, int ownerId)
        {
            return await _context.Restaurants
                .Include(r => r.Owner)
                .Include(r => r.MenuItems)
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId &&
                                        r.OwnerId == ownerId);
        }
    }
}
