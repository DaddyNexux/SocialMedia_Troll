namespace SocialMedia.Models.DTOs.UserrManagment
{
    public class LoginDTO
    {
        public string? Username{ get; set; }
        public string? Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public Guid profileId { get; set; }
        
    }

}
