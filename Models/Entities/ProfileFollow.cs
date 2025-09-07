using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
    public class ProfileFollow : BaseEntity
    {
        [ForeignKey("FollowerProfile")]
        public Guid FollowerProfileId { get; set; }
        public Profile? FollowerProfile { get; set; }
        [ForeignKey("FollowingProfile")]
        public Guid FollowingProfileId { get; set; }
        public Profile? FollowingProfile { get; set; }
    }
}
