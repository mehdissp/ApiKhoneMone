using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities.Blogs
{
    public class Comment
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } // نام نویسنده نظر
        public string Email { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }

        // کلید خارجی برای پست
        public int PostId { get; set; }
        public Post Post { get; set; }

        // خودارجاعی برای پاسخ به نظر (نested comment)
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment>? Replies { get; set; }
    }
}
