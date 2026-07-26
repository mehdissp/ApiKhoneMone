using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstate
{
    public class FilterRealEstateAllDto
    {
        // پارامترهای پایه
        public int TabId { get; set; } = 1;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // فیلتر مناطق (محله‌ها)
        public string? ChildIds { get; set; }  // "2,3,5,7"

        // فیلتر سال ساخت (چک‌باکس)
        public string? ConstructionYears { get; set; }  // "1400,1401,1402"

        // فیلتر محدوده قیمت
        public long? PriceMin { get; set; }
        public long? PriceMax { get; set; }

        // فیلتر محدوده متراژ
        public int? AreaMin { get; set; }
        public int? AreaMax { get; set; }

        // فیلتر محدوده سال ساخت
        public int? YearMin { get; set; }
        public int? YearMax { get; set; }

        // فیلتر محدوده طبقات
        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }

        // فیلتر محدوده اتاق
        public int? RoomMin { get; set; }
        public int? RoomMax { get; set; }

        // فیلتر امکانات
        public bool? IsHasElevator { get; set; }
        public bool? IsHasParking { get; set; }
        public bool? IsHasPool { get; set; }
        public bool? IsHasStoreRoom { get; set; }

        // مرتب‌سازی
        public string? SortBy { get; set; }  // "جدیدترین", "قدیمی‌ترین", "بیشترین امکانات"
        public int RegionId { get; set; }
    }
}
