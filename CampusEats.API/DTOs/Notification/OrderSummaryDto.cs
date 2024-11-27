namespace CampusEats.API.DTOs.Notification
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public string Status { get; set; }
        public DateTime OrderTime { get; set; }
    }
}
