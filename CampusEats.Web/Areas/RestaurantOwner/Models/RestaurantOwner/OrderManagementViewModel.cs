using static CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner.LiveOrdersViewModel;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
{
    public class OrderManagementViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new();
        public decimal TodayRevenue => Orders.Where(o => o.Status != "Cancelled")
                                            .Sum(o => o.TotalAmount);
        public int TotalOrders => Orders.Count;
        public int PendingOrders => Orders.Count(o => o.Status == "Pending");
    }
}
