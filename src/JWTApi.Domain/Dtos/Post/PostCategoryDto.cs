using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Post
{
    public class PostCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه
        public string CategoryPostName { get; set; } // محتوای اصلی (می‌تونه HTML باشه)
        public string? ImageUrl { get; set; } // ع
        public int? CountView { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
        public bool IsPublished { get; set; }
    }
}
