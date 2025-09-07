namespace SocialMedia.Mappers
{
    using SocialMedia.Models.DTOs;
    using SocialMedia.Models.DTOs.UserrManagment;
    using SocialMedia.Models.Entities;

    public static class PostMapper
    {
        // Post → PostDTO
        public static PostDTO ToDTO(this Post post)
        {
            if (post == null) return null!;

            return new PostDTO
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                CreatedAt = post.PostedAt,
                Author = post.Profile?.ToDTO(),
                Comments = post.Comments?.Select(c => c.ToDTO()).ToList()
            };
        }

        // Comment → CommentDTO
        public static CommentDTO ToDTO(this PostComments comment)
        {
            if (comment == null) return null!;

            return new CommentDTO
            {
                Content = comment.Content,
                CreatedAt = comment.CommentedAt,
                Author = comment.Profile?.ToDTO(),
                Post = null // prevent circular references (optional)
            };
        }

        // Profile → ProfileDTO
        public static ProfileDTO ToDTO(this Profile profile)
        {
            if (profile == null) return null!;

            return new ProfileDTO
            {
                Id = profile.Id,
                Fullname = profile.FullName,
                Bio = profile.Bio,
                AvatarUrl = profile.AvatarUrl,
                FollowersCount = profile.FollowersCount,
                FollowingCount = profile.FollowingCount,
                PostsCount = profile.PostsCount,
                User = profile.User?.ToDTO()
            };
        }

        // User → UserDTO
        public static UserDTO ToDTO(this User user)
        {
            if (user == null) return null!;

            return new UserDTO
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
            };
        }
    }
}
