namespace CampusEats.Web.Areas.Admin.Models.Restaurant
{
    public class RestaurantViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
