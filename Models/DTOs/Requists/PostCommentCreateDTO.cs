namespace SocialMedia.Models.DTOs.Requists
{
    public class PostCommentCreateDTO 
    {
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public Guid ProfileId { get; set; }

    }

}
