namespace CampusEats.API.DTOs.User
{
    public class OrderItemDetailDto
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
