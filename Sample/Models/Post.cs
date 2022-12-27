using System.Collections.Generic;

namespace efCoreSeederSample.Models
{
    public class Post
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }

        public Category Category { get; set; }
        public List<Comment> Comments { get; set; }
    }
}