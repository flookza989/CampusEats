namespace CampusEats.Web.Areas.RestaurantOwner.Models.Orders
{
    public class LiveOrdersViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<OrderGroup> OrderGroups { get; set; }

        public class OrderGroup
        {
            public string Status { get; set; }
            public List<OrderViewModel> Orders { get; set; }
        }

        public class OrderViewModel
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; }
            public DateTime OrderTime { get; set; }
            public string Status { get; set; }
            public decimal TotalAmount { get; set; }
            public List<OrderItemViewModel> Items { get; set; }
        }
    }
}
