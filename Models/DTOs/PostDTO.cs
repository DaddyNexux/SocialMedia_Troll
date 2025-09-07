namespace SocialMedia.Models.DTOs
{
    public class PostDTO : BaseDTO
    {
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public ProfileDTO? Author { get; set; }
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;

        public ICollection<CommentDTO>? Comments { get; set; }
    }
}
