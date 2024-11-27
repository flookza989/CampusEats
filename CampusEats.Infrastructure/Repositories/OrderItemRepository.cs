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
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByMenuItemAsync(int menuItemId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.ItemId == menuItemId)
                .OrderByDescending(oi => oi.Order.OrderTime)
                .ToListAsync();
        }

        public async Task UpdateQuantityAsync(int orderItemId, int quantity)
        {
            var orderItem = await GetByIdAsync(orderItemId);
            if (orderItem != null)
            {
                orderItem.Quantity = quantity;
                await UpdateAsync(orderItem);
            }
        }

        public override async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsWithDetailsAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<decimal> GetOrderItemTotalAsync(int orderItemId)
        {
            var orderItem = await GetByIdAsync(orderItemId);
            return orderItem != null ? orderItem.UnitPrice * orderItem.Quantity : 0;
        }
    }
}
