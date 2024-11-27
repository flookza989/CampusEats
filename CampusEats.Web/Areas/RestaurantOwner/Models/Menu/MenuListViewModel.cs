namespace CampusEats.Web.Areas.RestaurantOwner.Models.Menu
{
    public class MenuListViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<MenuItemViewModel> MenuItems { get; set; }
    }
}
