namespace CampusEats.Web.Models.Restaurant
{
    public class RestaurantListViewModel
    {
        public List<RestaurantSummaryViewModel> Restaurants { get; set; }
        public string SearchTerm { get; set; }
    }
}
