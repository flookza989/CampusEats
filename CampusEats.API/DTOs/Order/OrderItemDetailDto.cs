namespace CampusEats.API.DTOs.Order
{
    public class OrderItemDetailDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
