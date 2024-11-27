namespace CampusEats.Web.Models.Restaurant
{
    public class RestaurantSummaryViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsOpen { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public int MenuItemCount { get; set; }
        public List<MenuItemViewModel> FeaturedItems { get; set; }
    }
}
