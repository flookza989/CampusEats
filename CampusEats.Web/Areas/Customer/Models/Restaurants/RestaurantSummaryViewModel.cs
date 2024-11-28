namespace CampusEats.Web.Areas.Customer.Models.Restaurants
{
    public class RestaurantSummaryViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsOpen { get; set; }
        public string ImageUrl { get; set; }
        public string OpeningTimeString { get; set; }
        public string ClosingTimeString { get; set; }
    }
}
