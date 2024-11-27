using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateOrderStatusNotificationAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            var message = GenerateStatusNotificationMessage(status);
            var notification = new Notification
            {
                UserId = order.UserId,
                OrderId = orderId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Status = status
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            return await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAllNotificationsAsReadAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendOrderReadyNotificationAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            var notification = new Notification
            {
                UserId = order.UserId,
                OrderId = orderId,
                Message = "Your order is ready for pickup!",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
            return notifications.Count();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            await _unitOfWork.Notifications.DeleteAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateStatusNotificationMessage(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Your order has been received and is pending confirmation.",
                OrderStatus.Preparing => "Your order is being prepared.",
                OrderStatus.Ready => "Your order is ready for pickup!",
                OrderStatus.Completed => "Your order has been completed. Enjoy your meal!",
                OrderStatus.Cancelled => "Your order has been cancelled.",
                _ => $"Order status updated to: {status}"
            };
        }
    }
}
