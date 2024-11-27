using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRestaurantRepository Restaurants { get; }
        IMenuItemRepository MenuItems { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        INotificationRepository Notifications { get; }

        Task<bool> SaveChangesAsync();
    }
}
