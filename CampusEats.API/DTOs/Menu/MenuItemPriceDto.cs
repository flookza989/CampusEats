using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Menu
{
    public class MenuItemPriceDto
    {
        [Required]
        [Range(0.01, 10000)]
        public decimal NewPrice { get; set; }
    }
}
