using Microsoft.EntityFrameworkCore;

namespace efCoreSeederSample.Models
{
    public class SeederSampleDbContext : DbContext
    {
        public SeederSampleDbContext(DbContextOptions options) : base(options)
        {
        }

        protected SeederSampleDbContext()
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // For post
            modelBuilder.Entity<Post>()
                .HasOne(post => post.Category)
                .WithMany(category => category.Posts)
                .HasForeignKey(post => post.CategoryId);

            // For comment
            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Post)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.PostId);
        }
    }
}