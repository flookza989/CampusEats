namespace CampusEats.Web.Areas.Customer.Models.Restaurants
{
    public class MenuItemViewModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
