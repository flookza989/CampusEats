using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public List<OrderItemCreateDto> Items { get; set; }

        public string SpecialInstructions { get; set; }
    }
}
