namespace CampusEats.Web.Areas.RestaurantOwner.Models.Orders
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string SpecialInstructions { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();

        public string StatusBadgeClass => Status.ToLower() switch
        {
            "pending" => "bg-warning",
            "preparing" => "bg-info",
            "ready" => "bg-success",
            "completed" => "bg-primary",
            "cancelled" => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
