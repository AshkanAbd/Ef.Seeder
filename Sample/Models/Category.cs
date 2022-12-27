using System.Collections.Generic;

namespace efCoreSeederSample.Models
{
    public class Category 
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<Post> Posts { get; set; }
    }
}