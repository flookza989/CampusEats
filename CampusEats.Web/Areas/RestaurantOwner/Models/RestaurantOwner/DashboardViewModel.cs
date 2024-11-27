using CampusEats.Core.Entities;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
{
    public class DashboardViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<Order> TodayOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int MenuItemCount { get; set; }
        public int PendingOrders { get; set; }

        public bool IsOpen =>
            Restaurant.IsActive &&
            DateTime.Now.TimeOfDay >= Restaurant.OpeningTime &&
            DateTime.Now.TimeOfDay <= Restaurant.ClosingTime;
    }
}
