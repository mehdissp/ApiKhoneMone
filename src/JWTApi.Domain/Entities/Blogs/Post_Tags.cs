using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities.Blogs
{
    public class Post_Tags
    {
        public int PostId { get; set; }
        public int TagsId { get; set; }
    }
}
