using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Areas.Admin.Models.User
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public string UserType { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? NewPassword { get; set; }
    }
}
