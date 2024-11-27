using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Notification
{
    public class CreateNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }
    }
}
