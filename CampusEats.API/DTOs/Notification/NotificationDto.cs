using CampusEats.API.DTOs.Order;

namespace CampusEats.API.DTOs.Notification
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderSummaryDto Order { get; set; }
    }
}
