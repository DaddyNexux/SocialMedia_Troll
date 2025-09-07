using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
    public class PostComments : BaseEntity
    {
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("Post")]
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public Profile? Profile { get; set; }
    }
}
