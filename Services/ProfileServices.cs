using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Mappers;
using SocialMedia.Models.DTOs;
using SocialMedia.Models.DTOs.Requists;
using SocialMedia.Models.Entities;

namespace SocialMedia.Services
{
    public interface IProfileService
    {
        Task<ProfileDTO?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProfileDTO>> GetAllAsync();
        Task<ProfileDTO?> UpdateAsync(Guid id, ProfileUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ProfileDTO>> SearchAsync(string query);


    }

    public class ProfileService : IProfileService
    {
        private readonly AppData _context;

        public ProfileService(AppData context)
        {
            _context = context;
        }

        public async Task<ProfileDTO?> GetByIdAsync(Guid id)
        {
            var profile = await _context.Profiles
                .Include(p => p.User) // load user data if needed
                .FirstOrDefaultAsync(p => p.Id == id);

            return profile?.ToDTO();
        }

        public async Task<IEnumerable<ProfileDTO>> GetAllAsync()
        {
            var profiles = await _context.Profiles
                .Include(p => p.User)
                .ToListAsync();

            return profiles.Select(p => p.ToDTO());
        }

        public async Task<ProfileDTO?> UpdateAsync(Guid id, ProfileUpdateDTO dto)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null) return null;

            if (!string.IsNullOrEmpty(dto.Name))
                profile.FullName = dto.Name;

            if (!string.IsNullOrEmpty(dto.Bio))
                profile.Bio = dto.Bio;

            if (!string.IsNullOrEmpty(dto.AvatarUrl))
                profile.AvatarUrl = dto.AvatarUrl;

            await _context.SaveChangesAsync();
            return profile.ToDTO();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null) return false;

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProfileDTO>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<ProfileDTO>();

            query = query.ToLower();

            var profiles = await _context.Profiles
                .Include(p => p.User) // so we can search by Username too
                .Where(p => p.FullName.ToLower().Contains(query) ||
                            p.User.UserName.ToLower().Contains(query))
                .ToListAsync();

            return profiles.Select(p => p.ToDTO());
        }

    }
}
