using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Models.Auth
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        public string UserType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
