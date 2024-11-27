using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Areas.RestaurantOwner.Models.Menu
{
    public class CreateMenuItemViewModel
    {
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
        public decimal Price { get; set; }

        public IFormFile Image { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
