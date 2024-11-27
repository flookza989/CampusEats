namespace CampusEats.Web.Models.Restaurant
{
    public class RestaurantDetailViewModel
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public bool IsOpen { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public List<MenuItemViewModel> MenuItems { get; set; }
    }
}
