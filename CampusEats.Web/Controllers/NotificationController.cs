using CampusEats.Core.Interfaces;
using CampusEats.Web.Hubs;
using CampusEats.Web.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CampusEats.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(GetCurrentUserId());
            var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(GetCurrentUserId());

            var viewModel = new NotificationsViewModel
            {
                Notifications = notifications.Select(n => new NotificationItemViewModel
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    Type = NotificationType.OrderStatus,
                    OrderId = n.OrderId,
                    OrderStatus = n.Status.ToString(),
                    RestaurantName = n.Order.Restaurant.Name
                }).ToList(),
                UnreadCount = unreadCount
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkNotificationAsReadAsync(id);
            var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(GetCurrentUserId());

            // Update badge count via SignalR
            await _hubContext.Clients
                .Group($"User_{GetCurrentUserId()}")
                .SendAsync("UpdateNotificationCount", unreadCount);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllNotificationsAsReadAsync(GetCurrentUserId());

            // Update badge count via SignalR
            await _hubContext.Clients
                .Group($"User_{GetCurrentUserId()}")
                .SendAsync("UpdateNotificationCount", 0);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadNotificationCountAsync(GetCurrentUserId());
            return Json(new { count });
        }

        public async Task<IActionResult> GetNotificationPartial()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(GetCurrentUserId());
            var viewModel = notifications.Select(n => new NotificationItemViewModel
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                Type = NotificationType.OrderStatus,
                OrderId = n.OrderId,
                OrderStatus = n.Status.ToString(),
                RestaurantName = n.Order.Restaurant.Name
            }).ToList();

            return PartialView("_NotificationsList", viewModel);
        }
    }
}
