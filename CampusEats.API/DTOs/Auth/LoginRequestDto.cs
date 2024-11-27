using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
