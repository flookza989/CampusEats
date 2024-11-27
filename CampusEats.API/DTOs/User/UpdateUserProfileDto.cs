using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.User
{
    public class UpdateUserProfileDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}
