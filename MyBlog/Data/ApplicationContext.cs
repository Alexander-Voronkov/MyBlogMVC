using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data.Entities;

namespace MyBlog.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<PostImage> PostImages  { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        //public DbSet<User> Users { get; set; } = default!;
    }
}
