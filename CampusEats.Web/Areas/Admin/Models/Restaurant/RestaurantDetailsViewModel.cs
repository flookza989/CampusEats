using CampusEats.Web.Areas.Admin.Models.User;

namespace CampusEats.Web.Areas.Admin.Models.Restaurant
{
    public class RestaurantDetailsViewModel : RestaurantViewModel
    {
        public UserViewModel Owner { get; set; }
        public int MenuItemCount { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
