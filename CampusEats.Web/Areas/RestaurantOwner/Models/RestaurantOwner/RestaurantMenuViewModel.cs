
using CampusEats.Web.Areas.RestaurantOwner.Models.Menu;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
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
