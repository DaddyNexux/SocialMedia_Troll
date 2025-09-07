namespace SocialMedia.Models.DTOs
{
    public class MessagesDTO : BaseDTO
    {
        public string? Content { get; set; }
        public ProfileDTO? Sender { get; set; }
        public ProfileDTO? Receiver { get; set; }
        
    }
}
