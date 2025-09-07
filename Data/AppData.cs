using SocialMedia.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Data
{
    public class AppData : IdentityDbContext<User, Role, Guid>
    {
        public AppData(DbContextOptions<AppData> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }
        public DbSet<ProfileFollow > ProfileFollows { get; set; }
        public DbSet<DMs> DMs { get; set; }




    }
}
