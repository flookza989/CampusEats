using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Menu
{
    public class CreateMenuItemDto
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
