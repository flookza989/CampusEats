namespace CampusEats.API.DTOs.User
{
    public class UserOrderHistoryDto
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDetailDto> Items { get; set; }
    }
}
