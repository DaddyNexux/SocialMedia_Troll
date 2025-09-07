namespace SocialMedia.Models.DTOs.Requists
{
    public class ProfileCreateDTO 
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
    }
    public class ProfileUpdateDTO
    {
        public string? Username { get; set; }

        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class UpdatePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
