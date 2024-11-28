namespace CampusEats.Web.Areas.Customer.Models.Carts
{
    public class AddToCartRequest
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
