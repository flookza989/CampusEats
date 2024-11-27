using CampusEats.Web.Areas.Admin.Models.User;
using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Areas.Admin.Models.Restaurant
{
    public class RestaurantEditViewModel
    {
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Opening time is required")]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "Closing time is required")]
        public TimeSpan ClosingTime { get; set; }

        public bool IsActive { get; set; }
        public int OwnerId { get; set; }
        public UserViewModel? Owner { get; set; }
    }
}
