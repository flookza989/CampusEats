namespace CampusEats.Web.Models.Order
{
    public class OrderItemViewModel
    {
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }
}
