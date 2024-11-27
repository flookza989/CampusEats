using CampusEats.API.DTOs.User;

namespace CampusEats.API.DTOs.Order
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string SpecialInstructions { get; set; }
        public List<OrderItemDetailDto> Items { get; set; }
    }
}
