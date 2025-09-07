using SocialMedia.Models.DTOs.UserrManagment;

namespace SocialMedia.Models.DTOs
{
    public class ProfileDTO : BaseDTO
    {
        public string? Fullname { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public UserDTO? User { get; set; }
        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        public int PostsCount { get; set; } = 0;

    }


   
    
}
