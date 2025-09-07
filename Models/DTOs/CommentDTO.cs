namespace SocialMedia.Models.DTOs
{
    public class CommentDTO
    {
        public string? Content { get; set; }
        public ProfileDTO? Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public PostDTO? Post { get; set; }
    }
}
