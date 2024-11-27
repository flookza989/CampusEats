using CampusEats.API.DTOs.Notification;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;

        public NotificationController(
            INotificationService notificationService,
            IOrderService orderService)
        {
            _notificationService = notificationService;
            _orderService = orderService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);

                var notificationDtos = notifications.Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    OrderId = n.OrderId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Order = new OrderSummaryDto
                    {
                        OrderId = n.Order.OrderId,
                        RestaurantName = n.Order.Restaurant.Name,
                        Status = n.Order.Status.ToString(),
                        OrderTime = n.Order.OrderTime
                    }
                });

                return Ok(notificationDtos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notifications." });
            }
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<NotificationCountDto>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _notificationService.GetUnreadNotificationCountAsync(userId);

                return Ok(new NotificationCountDto { UnreadCount = count });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving unread count." });
            }
        }

        [HttpPost("{id}/mark-read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkNotificationAsReadAsync(id);
                return Ok(new { message = "Notification marked as read" });
            }
            catch (NotificationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _notificationService.MarkAllNotificationsAsReadAsync(userId);
                return Ok(new { message = "All notifications marked as read" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while marking notifications as read." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(id);
                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (NotificationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
