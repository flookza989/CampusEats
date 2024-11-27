using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Order
{
    public class OrderItemCreateDto
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        public string SpecialInstructions { get; set; }
    }
}
