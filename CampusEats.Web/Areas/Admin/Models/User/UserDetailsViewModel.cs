using CampusEats.Web.Areas.Admin.Models.User;

namespace CampusEats.Web.Areas.Admin.Models.User
{
    public class UserDetailsViewModel : UserViewModel
    {
        public List<OrderSummaryViewModel> Orders { get; set; }
    }
}
