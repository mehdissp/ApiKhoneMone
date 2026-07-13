using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities.Blogs
{
    public class CategoryPost
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // برای URL های سئو شده

        public ICollection<Post> Posts { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}
