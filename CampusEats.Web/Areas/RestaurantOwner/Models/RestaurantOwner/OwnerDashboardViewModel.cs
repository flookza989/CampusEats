using CampusEats.Core.Entities;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
{
    public class OwnerDashboardViewModel
    {
        public Restaurant Restaurant { get; set; }
        public IEnumerable<Order> PendingOrders { get; set; }
        public int TotalOrdersToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public int ActiveMenuItems { get; set; }
    }
}
