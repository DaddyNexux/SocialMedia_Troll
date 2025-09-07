using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Extentions;
using SocialMedia.Helpers;
using SocialMedia.Mappers;
using SocialMedia.Models.DTOs;
using SocialMedia.Models.DTOs.Common;
using SocialMedia.Models.Entities;

namespace SocialMedia.Services.UserActivities
{
    public interface IPostServices
    {
        Task<ApiResponse<PostDTO>> CreatePost(Guid profileId, string content, string? imageUrl);
        Task<ApiResponse<PagedList<PostDTO>>> GetAllPosts(BaseFilter filter);
        Task<ApiResponse<PagedList<PostDTO>>> GetPostsByProfile(Guid profileId, BaseFilter filter);
        Task<ApiResponse<string>> LikePost(Guid profileId, Guid postId);
        Task<ApiResponse<string>> UnlikePost(Guid profileId, Guid postId);
        Task<ApiResponse<CommentDTO>> AddComment(Guid profileId, Guid postId, string content);
        Task<ApiResponse<PagedList<CommentDTO>>> GetComments(Guid postId, BaseFilter filter);
    }

    public class PostServices : IPostServices
    {
        private readonly AppData _context;

        public PostServices(AppData context)
        {
            _context = context;
        }

        // ✅ Create Post
        public async Task<ApiResponse<PostDTO>> CreatePost(Guid profileId, string content, string? imageUrl)
        {
            var profile = await _context.Profiles.FindAsync(profileId);
            if (profile == null) return ApiResponse<PostDTO>.Fail("Profile not found", 404);

            var post = new Post
            {
                ProfileId = profileId,
                Content = content,
                ImageUrl = imageUrl,
                PostedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            profile.PostsCount++;
            await _context.SaveChangesAsync();

            return ApiResponse<PostDTO>.Success(post.ToDTO(), "Post created successfully", 201);
        }

        // ✅ Get all posts with pagination
        public async Task<ApiResponse<PagedList<PostDTO>>> GetAllPosts(BaseFilter filter)
        {
            var query = _context.Posts
                .Include(p => p.Profile)
                .Include(p => p.Comments) // so CommentsCount maps correctly
                .OrderByDescending(p => p.PostedAt)
                .WhereBaseFilter(filter);

            var pagedPosts = await query.Paginate(filter);

            var mapped = new PagedList<PostDTO>(
                pagedPosts.Items.Select(p => p.ToDTO()).ToList(),
                pagedPosts.PageNumber,
                pagedPosts.PageSize,
                pagedPosts.TotalCount
            );

            return ApiResponse<PagedList<PostDTO>>.Success(mapped, "Posts retrieved successfully");
        }

        // ✅ Get posts by profile with pagination
        public async Task<ApiResponse<PagedList<PostDTO>>> GetPostsByProfile(Guid profileId, BaseFilter filter)
        {
            var query = _context.Posts
                .Where(p => p.ProfileId == profileId)
                .Include(p => p.Profile)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.PostedAt)
                .WhereBaseFilter(filter);

            var pagedPosts = await query.Paginate(filter);

            var mapped = new PagedList<PostDTO>(
                pagedPosts.Items.Select(p => p.ToDTO()).ToList(),
                pagedPosts.PageNumber,
                pagedPosts.PageSize,
                pagedPosts.TotalCount
            );

            return ApiResponse<PagedList<PostDTO>>.Success(mapped, "User posts retrieved successfully");
        }

        // ✅ Like Post
        public async Task<ApiResponse<string>> LikePost(Guid profileId, Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return ApiResponse<string>.Fail("Post not found", 404);

            bool alreadyLiked = await _context.PostLikes
                .AnyAsync(l => l.PostId == postId && l.ProfileId == profileId);

            if (alreadyLiked)
                return ApiResponse<string>.Fail("You already liked this post", 400);

            var like = new PostLikes { PostId = postId, ProfileId = profileId };
            _context.PostLikes.Add(like);
            post.LikesCount++;
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Success("Liked successfully");
        }

        // ✅ Unlike Post
        public async Task<ApiResponse<string>> UnlikePost(Guid profileId, Guid postId)
        {
            var like = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.ProfileId == profileId);

            if (like == null)
                return ApiResponse<string>.Fail("You haven't liked this post", 400);

            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.LikesCount > 0)
                post.LikesCount--;

            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Success("Unliked successfully");
        }

        // ✅ Add Comment
        public async Task<ApiResponse<CommentDTO>> AddComment(Guid profileId, Guid postId, string content)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return ApiResponse<CommentDTO>.Fail("Post not found", 404);

            var comment = new PostComments
            {
                PostId = postId,
                ProfileId = profileId,
                Content = content,
                CommentedAt = DateTime.UtcNow
            };

            _context.PostComments.Add(comment);
            post.CommentsCount++;
            await _context.SaveChangesAsync();

            return ApiResponse<CommentDTO>.Success(comment.ToDTO(), "Comment added successfully", 201);
        }

        // ✅ Get Comments of a Post with pagination
        public async Task<ApiResponse<PagedList<CommentDTO>>> GetComments(Guid postId, BaseFilter filter)
        {
            var query = _context.PostComments
                .Where(c => c.PostId == postId)
                .Include(c => c.Profile)
                .OrderByDescending(c => c.CommentedAt)
                .WhereBaseFilter(filter);

            var pagedComments = await query.Paginate(filter);

            var mapped = new PagedList<CommentDTO>(
                pagedComments.Items.Select(c => c.ToDTO()).ToList(),
                pagedComments.PageNumber,
                pagedComments.PageSize,
                pagedComments.TotalCount
            );

            return ApiResponse<PagedList<CommentDTO>>.Success(mapped, "Comments retrieved successfully");
        }
    }
}
