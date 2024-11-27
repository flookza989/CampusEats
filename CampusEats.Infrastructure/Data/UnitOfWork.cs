using CampusEats.Core.Interfaces;
using CampusEats.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository _users;
        private IRestaurantRepository _restaurants;
        private IMenuItemRepository _menuItems;
        private IOrderRepository _orders;
        private IOrderItemRepository _orderItems;
        private INotificationRepository _notifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IRestaurantRepository Restaurants =>
            _restaurants ??= new RestaurantRepository(_context);

        public IMenuItemRepository MenuItems =>
            _menuItems ??= new MenuItemRepository(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        public IOrderItemRepository OrderItems =>
            _orderItems ??= new OrderItemRepository(_context);

        public INotificationRepository Notifications =>
            _notifications ??= new NotificationRepository(_context);

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
