using SocialMedia.Models.DTOs.Common;
using SocialMedia.Models.DTOs.Requists;
using SocialMedia.Models.DTOs;
using SocialMedia.Data;
using SocialMedia.Models.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Mappers;
using SocialMedia.Helpers;
using SocialMedia.Extentions;

namespace SocialMedia.Services.UserActivities
{
    public interface IDMService
    {
        Task<ApiResponse<MessagesDTO>> SendMessage(Guid senderProfileId, DMsCreateDTO dto);
        Task<ApiResponse<PagedList<MessagesDTO>>> GetConversationPaged(Guid profileAId, Guid profileBId, BaseFilter filter);
        Task<ApiResponse<PagedList<MessagesDTO>>> GetAllMessagesPaged(Guid profileId, BaseFilter filter);
        Task<ApiResponse<IEnumerable<MessagesDTO>>> GetInbox(Guid profileId);


    }
    public class DMService : IDMService
    {
        private readonly AppData _context;

        public DMService(AppData context)
        {
            _context = context;
        }

        // ✅ Send a message
        public async Task<ApiResponse<MessagesDTO>> SendMessage(Guid senderProfileId, DMsCreateDTO dto)
        {
            if (dto.ReceiverId == null)
                return ApiResponse<MessagesDTO>.Fail("Receiver ID is required", 400);

            var sender = await _context.Profiles.FindAsync(senderProfileId);
            var receiver = await _context.Profiles.FindAsync(dto.ReceiverId.Value);

            if (sender == null || receiver == null)
                return ApiResponse<MessagesDTO>.Fail("Sender or receiver profile not found", 404);

            var message = new DMs
            {
                SenderProfileId = senderProfileId,
                ReceiverProfileId = dto.ReceiverId.Value,
                Message = dto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.DMs.Add(message);
            await _context.SaveChangesAsync();

            return ApiResponse<MessagesDTO>.Success(message.ToDTO(), "Message sent successfully", 201);
        }

        public async Task<ApiResponse<PagedList<MessagesDTO>>> GetConversationPaged(Guid profileAId, Guid profileBId, BaseFilter filter)
        {
            var query = _context.DMs
                .Where(dm => (dm.SenderProfileId == profileAId && dm.ReceiverProfileId == profileBId) ||
                             (dm.SenderProfileId == profileBId && dm.ReceiverProfileId == profileAId))
                .Include(dm => dm.SenderProfile)
                .Include(dm => dm.ReceiverProfile)
                .OrderBy(dm => dm.SentAt)
                .AsQueryable();

            var paged = await query.Paginate(filter); // uses your existing IQueryableExtensions and PagedList

            // Map to DTO
            var pagedDto = new PagedList<MessagesDTO>(
                paged.Items.Select(m => m.ToDTO()).ToList(),
                paged.PageNumber,
                paged.PageSize,
                paged.TotalCount
            );

            return ApiResponse<PagedList<MessagesDTO>>.Success(pagedDto, "Conversation retrieved successfully");
        }

        public async Task<ApiResponse<PagedList<MessagesDTO>>> GetAllMessagesPaged(Guid profileId, BaseFilter filter)
        {
            var query = _context.DMs
                .Where(dm => dm.SenderProfileId == profileId || dm.ReceiverProfileId == profileId)
                .Include(dm => dm.SenderProfile)
                .Include(dm => dm.ReceiverProfile)
                .OrderBy(dm => dm.SentAt)
                .AsQueryable();

            var paged = await query.Paginate(filter);

            var pagedDto = new PagedList<MessagesDTO>(
                paged.Items.Select(m => m.ToDTO()).ToList(),
                paged.PageNumber,
                paged.PageSize,
                paged.TotalCount
            );

            return ApiResponse<PagedList<MessagesDTO>>.Success(pagedDto, "Messages retrieved successfully");
        }
        public async Task<ApiResponse<IEnumerable<MessagesDTO>>> GetInbox(Guid profileId)
        {
            // Get all messages where the user is either sender or receiver
            var messages = await _context.DMs
                .Where(dm => dm.SenderProfileId == profileId || dm.ReceiverProfileId == profileId)
                .Include(dm => dm.SenderProfile)
                .Include(dm => dm.ReceiverProfile)
                .ToListAsync();

            // Group by the "other" profile (conversation partner)
            var latestMessages = messages
                .GroupBy(dm => dm.SenderProfileId == profileId ? dm.ReceiverProfileId : dm.SenderProfileId)
                .Select(g => g.OrderByDescending(dm => dm.SentAt).First()) // take latest message in each conversation
                .OrderByDescending(dm => dm.SentAt) // order by latest message time
                .ToList();

            var inboxDto = latestMessages.Select(m => m.ToDTO());

            return ApiResponse<IEnumerable<MessagesDTO>>.Success(inboxDto, "Inbox retrieved successfully");
        }


    }
}
