namespace CampusEats.Web.Areas.Customer.Models.Restaurants
{
    public class RestaurantDetailViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public bool IsOpen { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public List<MenuCategoryViewModel> Categories { get; set; } = new();
    }
}
