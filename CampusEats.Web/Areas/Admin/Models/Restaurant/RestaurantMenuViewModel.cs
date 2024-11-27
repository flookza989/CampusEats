using CampusEats.Web.Areas.Admin.Models.Menu;

namespace CampusEats.Web.Areas.Admin.Models.Restaurant
{
    public class RestaurantMenuViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<MenuItemViewModel> MenuItems { get; set; } = new();
        public int TotalItems => MenuItems.Count;
        public int AvailableItems => MenuItems.Count(x => x.IsAvailable);
    }
}
