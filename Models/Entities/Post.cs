using SocialMedia.Models.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
    public class Post : BaseEntity
    {
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public Profile? Profile { get; set; }

        public ICollection<PostComments>? Comments { get; set; }

    }
}
