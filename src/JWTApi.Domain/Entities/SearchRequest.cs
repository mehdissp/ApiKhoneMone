using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class SearchRequest
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        // محدوده‌های عددی
        public int? MinSquareMeter { get; set; }
        public int? MaxSquareMeter { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? MinFloor { get; set; }
        public int? MaxFloor { get; set; }
        public int? MinRoomCount { get; set; }
        public int? MaxRoomCount { get; set; }
        public bool LowUnitCount { get; set; } = false;  // کم واحد باشه
        public int CategoryId { get; set; }                                            // وضعیت و زمان
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMatchDate { get; set; }
        public Category Category { get; set; } = default!;
        public ICollection<SearchMatch> Matches { get; set; } = new List<SearchMatch>();

    }
}
