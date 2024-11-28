namespace CampusEats.Web.Areas.Customer.Models.Carts
{
    public class ShoppingCart
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(item => item.Total);
    }
}
