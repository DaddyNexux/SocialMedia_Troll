namespace SocialMedia.Models.DTOs
{
    public class FollowersDTO : BaseDTO
    {
        public ProfileDTO? Follower { get; set; }
        public ProfileDTO? Following { get; set; }
    }
}
