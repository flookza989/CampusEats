namespace CampusEats.API.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
