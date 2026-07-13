using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Posts
{
    public class PostDtos
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه
        public string Content { get; set; } // محتوای اصلی (می‌تونه HTML باشه)
        public string? ImageUrl { get; set; } // عکس شاخص
        public bool IsPublished { get; set; }
        public string TempImageCacheIds { get; set; }
        public int[] TagsId { get; set; }
        public int? CategoryId { get; set; }
    }
}
