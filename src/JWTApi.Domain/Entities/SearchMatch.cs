using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JWTApi.Domain.Entities
{
    public class SearchMatch
    {
        public int Id { get; set; }
        // کلیدهای خارجی
        public int SearchRequestId { get; set; }
        public SearchRequest SearchRequest { get; set; } = default!;

        public int RealEstateId { get; set; }
   //     public RealEstates RealEstates { get; set; } = default!;


        public int RealEstateRentId { get; set; }
    //    public RealEstatesRent RealEstatesRent { get; set; } = default!;

        // امتیاز AI
        public double MatchScore { get; set; }      // امتیاز تطبیق ۰-۱۰۰

        // علت تطبیق
        public string MatchReasons { get; set; }    // JSON یا CSV دلایل: "متراژ,قیمت,منطقه"

        // وضعیت
        public bool IsNotified { get; set; } = false;        // اطلاع داده شده؟
        public bool IsSeenByUser { get; set; } = false;      // کاربر دیده؟
        public bool IsRejected { get; set; } = false;        // کاربر رد کرده؟

        // تاریخ‌ها
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public DateTime? NotifiedAt { get; set; }
        public DateTime? SeenAt { get; set; }
    }
}
