using SocialMedia.Models.DTOs.Common;
using SocialMedia.Models.DTOs;
using SocialMedia.Data;
using SocialMedia.Models.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Mappers;

namespace SocialMedia.Services.UserActivities
{
    public interface IProfileFollowService
    {
        Task<ApiResponse<string>> Follow(Guid followerId, Guid followingId);
        Task<ApiResponse<string>> Unfollow(Guid followerId, Guid followingId);
        Task<ApiResponse<IEnumerable<FollowersDTO>>> GetFollowers(Guid profileId);
        Task<ApiResponse<IEnumerable<FollowersDTO>>> GetFollowing(Guid profileId);
    }
    public class ProfileFollowService : IProfileFollowService
    {
        private readonly AppData _context;

        public ProfileFollowService(AppData context)
        {
            _context = context;
        }

        // ✅ Follow
        public async Task<ApiResponse<string>> Follow(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
                return ApiResponse<string>.Fail("You cannot follow yourself", 400);

            bool alreadyFollowing = await _context.ProfileFollows
                .AnyAsync(f => f.FollowerProfileId == followerId && f.FollowingProfileId == followingId);

            if (alreadyFollowing)
                return ApiResponse<string>.Fail("Already following this user", 400);

            var follow = new ProfileFollow
            {
                FollowerProfileId = followerId,
                FollowingProfileId = followingId
            };

            _context.ProfileFollows.Add(follow);

            // update counts
            var follower = await _context.Profiles.FindAsync(followerId);
            var following = await _context.Profiles.FindAsync(followingId);

            if (follower != null) follower.FollowingCount++;
            if (following != null) following.FollowersCount++;

            await _context.SaveChangesAsync();
            return ApiResponse<string>.Success("Followed successfully");
        }

        // ✅ Unfollow
        public async Task<ApiResponse<string>> Unfollow(Guid followerId, Guid followingId)
        {
            var follow = await _context.ProfileFollows
                .FirstOrDefaultAsync(f => f.FollowerProfileId == followerId && f.FollowingProfileId == followingId);

            if (follow == null)
                return ApiResponse<string>.Fail("You are not following this user", 400);

            _context.ProfileFollows.Remove(follow);

            // update counts
            var follower = await _context.Profiles.FindAsync(followerId);
            var following = await _context.Profiles.FindAsync(followingId);

            if (follower != null && follower.FollowingCount > 0) follower.FollowingCount--;
            if (following != null && following.FollowersCount > 0) following.FollowersCount--;

            await _context.SaveChangesAsync();
            return ApiResponse<string>.Success("Unfollowed successfully");
        }

        // ✅ Get Followers
        public async Task<ApiResponse<IEnumerable<FollowersDTO>>> GetFollowers(Guid profileId)
        {
            var followers = await _context.ProfileFollows
                .Where(f => f.FollowingProfileId == profileId)
                .Include(f => f.FollowerProfile)
                .Include(f => f.FollowingProfile)
                .ToListAsync();

            var mapped = followers.Select(f => new FollowersDTO
            {
                Follower = f.FollowerProfile?.ToDTO(),
                Following = f.FollowingProfile?.ToDTO()
            });

            return ApiResponse<IEnumerable<FollowersDTO>>.Success(mapped);
        }

        // ✅ Get Following
        public async Task<ApiResponse<IEnumerable<FollowersDTO>>> GetFollowing(Guid profileId)
        {
            var following = await _context.ProfileFollows
                .Where(f => f.FollowerProfileId == profileId)
                .Include(f => f.FollowerProfile)
                .Include(f => f.FollowingProfile)
                .ToListAsync();

            var mapped = following.Select(f => new FollowersDTO
            {
                Follower = f.FollowerProfile?.ToDTO(),
                Following = f.FollowingProfile?.ToDTO()
            });

            return ApiResponse<IEnumerable<FollowersDTO>>.Success(mapped);
        }
    }
}
