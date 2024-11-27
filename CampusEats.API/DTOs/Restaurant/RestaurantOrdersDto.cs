namespace CampusEats.API.DTOs.Restaurant
{
    public class RestaurantOrdersDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
