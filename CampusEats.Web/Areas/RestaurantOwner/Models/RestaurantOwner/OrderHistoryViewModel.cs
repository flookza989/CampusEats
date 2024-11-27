namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
{
    public class OrderHistoryViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new();
        public decimal TotalRevenue => Orders.Where(o => o.Status != "Cancelled")
                                           .Sum(o => o.TotalAmount);

        public Dictionary<string, int> OrderStatusSummary => Orders
            .GroupBy(o => o.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
