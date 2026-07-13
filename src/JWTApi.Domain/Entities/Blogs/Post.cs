using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JWTApi.Domain.Entities.Blogs
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه
        public string Content { get; set; } // محتوای اصلی (می‌تونه HTML باشه)
        public string? ImageUrl { get; set; } // عکس شاخص
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public int? ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // کلیدهای خارجی
        public Guid UserId { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public CategoryPost Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Tag> Tags { get; set; } // رابطه چند به چند

        public void create(string title,string slug,string summary,string content,string imgurl,bool ispublish,string userId)
        {
            Title=title;
            Slug=slug;
            Summary=summary;
            Content=content;
            ImageUrl=imgurl;
            IsPublished=ispublish;
            UserId = Guid.Parse(userId);

        }
    }
}
