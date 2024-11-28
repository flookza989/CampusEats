namespace CampusEats.Web.Areas.Customer.Models.Restaurants
{
    public class MenuCategoryViewModel
    {
        public string Name { get; set; }
        public List<MenuItemViewModel> Items { get; set; } = new();
    }
}
