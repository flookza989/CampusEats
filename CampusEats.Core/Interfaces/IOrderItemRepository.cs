using CampusEats.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByMenuItemAsync(int menuItemId);
        Task UpdateQuantityAsync(int orderItemId, int quantity);
    }
}
