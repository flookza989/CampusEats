using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner
{
    public class CreateRestaurantViewModel
    {
        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Opening time is required")]
        [Display(Name = "Opening Time")]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "Closing time is required")]
        [Display(Name = "Closing Time")]
        public TimeSpan ClosingTime { get; set; }
    }
}
