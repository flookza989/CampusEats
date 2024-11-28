namespace CampusEats.Web.Areas.Customer.Models.Carts
{
    public class CartViewModel
    {
        public ShoppingCart Cart { get; set; }
        public bool IsRestaurantOpen { get; set; }
        public string ErrorMessage { get; set; }
    }
}
