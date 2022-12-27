using System.Linq;
using Ef.Seeder.Attributes;
using efCoreSeederSample.Models;
using Microsoft.EntityFrameworkCore;

namespace efCoreSeederSample.Seed
{
    public class DatabaseSeed
    {
        public DatabaseSeed(SeederSampleDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public SeederSampleDbContext DbContext { get; set; }

        [Seeder(1, typeof(Category))]
        public void CategorySeeder()
        {
            for (var i = 1; i <= 3; i++) {
                DbContext.Categories.Add(new Category {
                    Name = $"Category {i}"
                });
            }

            DbContext.SaveChanges();
        }

        [Seeder(2, typeof(Post))]
        public void PostSeeder()
        {
            var categories = DbContext.Categories.ToList();

            categories.ForEach(category => {
                for (var i = 1; i <= 3; i++) {
                    DbContext.Posts.Add(new Post {
                        CategoryId = category.Id,
                        Title = $"Title for post {i} in category {category.Name}",
                        Description = $"Description for post {i} in category {category.Name}"
                    });
                }
            });

            DbContext.SaveChanges();
        }

        [Seeder(3, typeof(Comment))]
        public void CommentSeeder()
        {
            var posts = DbContext.Posts
                .Include(x => x.Category)
                .ToList();

            posts.ForEach(post => {
                for (var i = 1; i <= 3; i++) {
                    DbContext.Comments.Add(new Comment {
                        PostId = post.Id,
                        Content = $"Comment {i} for post {post.Id} in category {post.Category.Name}",
                    });
                }
            });

            DbContext.SaveChanges();
        }
    }
}