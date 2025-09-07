using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
    public class Profile : BaseEntity
    {
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }


        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        public int PostsCount { get; set; } = 0;

    }
}
