namespace CampusEats.Web.Models.Order
{
    public class CartItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public decimal Total => Price * Quantity;
    }
}
