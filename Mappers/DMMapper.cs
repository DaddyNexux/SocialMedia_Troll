using SocialMedia.Models.DTOs;
using SocialMedia.Models.Entities;

namespace SocialMedia.Mappers
{
    public static class DMMapper
    {
        public static MessagesDTO ToDTO(this DMs dm)
        {
            return new MessagesDTO
            {
                Id = dm.Id,
                Content = dm.Message,
                Sender = dm.SenderProfile?.ToDTO(),
                Receiver = dm.ReceiverProfile?.ToDTO()
            };
        }
    }
}
