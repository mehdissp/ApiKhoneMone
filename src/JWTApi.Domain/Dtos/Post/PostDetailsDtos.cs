using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Post
{
    public class PostDetailsDtos
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; } // خلاصه
        public string CategoryPostName { get; set; } // محتوای اصلی (می‌تونه HTML باشه)
        public string? ImageUrl { get; set; } // ع
        public int? CountView { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
        public Agent Agents { get; set; }
        public string[] Tags { get; set; }
    }

    public class Agent
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ConnectSocialMedia { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public bool HasStory { get; set; }
        public string UserId { get; set; }
    }
}
