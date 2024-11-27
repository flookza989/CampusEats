namespace CampusEats.Web.Models.Order
{
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public string StatusBadgeClass => GetStatusBadgeClass();

        private string GetStatusBadgeClass()
        {
            return Status.ToLower() switch
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
}
