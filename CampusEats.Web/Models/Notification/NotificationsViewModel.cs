namespace CampusEats.Web.Models.Notification
{
    public class NotificationsViewModel
    {
        public List<NotificationItemViewModel> Notifications { get; set; }
        public int UnreadCount { get; set; }
    }
}
