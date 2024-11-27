namespace CampusEats.Web.Models.Notification
{
    public class NotificationItemViewModel
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public int? OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string RestaurantName { get; set; }
    }

}
