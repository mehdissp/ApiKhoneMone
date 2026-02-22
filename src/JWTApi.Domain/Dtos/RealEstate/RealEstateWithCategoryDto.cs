using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstate
{
    // کلاس PagedResult با بهینه‌سازی حافظه
 

    // DTO با بهینه‌سازی حافظه
    [Serializable]
    public class RealEstateWithCategoryDto
    {
        public int Id { get; set; }
        public int? ConstructionYear { get; set; }
        public int? CountFloor { get; set; }
        public string Title { get; set; }
        public string AdditionalInformation { get; set; }
        public bool IsHasElevator { get; set; }
        public bool IsHasParking { get; set; }
        public bool IsHasPool { get; set; }
        public bool IsHasStoreRoom { get; set; }
        public string RegionName { get; set; }
        public string ParentName { get; set; }
        public string Address { get; set; }
        public int ImageCount { get; set; }
        public long Price { get; set; }
        public DateTime CreatedAt { get; set; }
        // پراپرتی جدید برای نمایش تاریخ به فرمت شمسی نسبی
        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
    }
}
