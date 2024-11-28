namespace CampusEats.Web.Areas.Customer.Models.Restaurants
{
    public class RestaurantListViewModel
    {
        public List<RestaurantSummaryViewModel> Restaurants { get; set; } = new();
        public string SearchTerm { get; set; }
        public bool ShowOpenOnly { get; set; }
        public int TotalRestaurants => Restaurants.Count;
        public int OpenRestaurants => Restaurants.Count(r => r.IsOpen);
    }
}
