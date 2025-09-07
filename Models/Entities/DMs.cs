using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
    public class DMs : BaseEntity
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("SenderProfile")]
        public Guid SenderProfileId { get; set; }
        public Profile? SenderProfile { get; set; }
        [ForeignKey("ReceiverProfile")]
        public Guid ReceiverProfileId { get; set; }
        public Profile? ReceiverProfile { get; set; }
    }
}
