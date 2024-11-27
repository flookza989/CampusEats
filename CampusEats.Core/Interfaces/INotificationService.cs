using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface INotificationService
    {
        Task CreateOrderStatusNotificationAsync(int orderId, OrderStatus status);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(int userId);
        Task SendOrderReadyNotificationAsync(int orderId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task DeleteNotificationAsync(int notificationId);
    }
}
