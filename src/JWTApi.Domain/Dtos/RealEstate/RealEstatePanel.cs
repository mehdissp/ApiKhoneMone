using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstate
{
    // DTO با بهینه‌سازی حافظه
    [Serializable]
    public class RealEstatePanel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public decimal? Price { get; set; }
        public int Area { get; set; }
        public int CountRooms { get; set; }
        public int CountFloor { get; set; }
        public int Floor { get; set; }
        public bool IsHasParking { get; set; }
        public bool IsHasElavator { get; set; }
        public bool IsHasLoan { get; set; }
        public string Status { get; set; }
        public string Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Images { get; set; }
        
        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
        
    }
}
