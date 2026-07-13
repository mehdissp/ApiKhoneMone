using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities.Blogs
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? CategoryId { get; set; }
        public CategoryPost Category { get; set; }
        public ICollection<Post> Posts { get; set; } // رابطه چند به چند
    }
}
