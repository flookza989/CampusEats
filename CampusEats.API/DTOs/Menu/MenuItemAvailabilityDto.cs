using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Menu
{
    public class MenuItemAvailabilityDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}
