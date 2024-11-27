using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Restaurant
{
    public class CreateRestaurantDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")]
        public string OpeningTime { get; set; }

        [Required]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")]
        public string ClosingTime { get; set; }
    }
}
