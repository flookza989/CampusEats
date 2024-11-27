namespace CampusEats.API.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
    }
}
