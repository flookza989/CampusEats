namespace CampusEats.API.DTOs.User
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
