namespace CampusEats.API.DTOs.Restaurant
{
    public class RestaurantDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public List<MenuItemDto> MenuItems { get; set; }
    }
}
