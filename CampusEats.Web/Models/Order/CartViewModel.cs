namespace CampusEats.Web.Models.Order
{
    public class CartViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal SubTotal => Items.Sum(i => i.Total);
        public string SpecialInstructions { get; set; }
    }
}
