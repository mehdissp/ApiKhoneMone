using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RealEstateAgentProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        // اطلاعات تخصصی مشاور املاک
        public string AgentCode { get; set; } = string.Empty; // کد مشاور
        public string? NationalCartNumber { get; set; } // کد ملی
        public string? OfficeAddress { get; set; } // آدرس دفتر
        public string? LicenseNumber { get; set; } // شماره پروانه
        public DateTime? LicenseExpiryDate { get; set; } // تاریخ اعتبار پروانه

        // مدیریت تعداد زیرمجموعه
        public int MaxSubAgents { get; set; } = 5; // حداکثر تعداد مشاور املاکی که می‌تواند ثبت کند
        public int CurrentSubAgentsCount { get; set; } = 0; // تعداد فعلی

        // آمار و رتبه
        public int AgentRank { get; set; } = 1; // رتبه (1 تا 5)
        public decimal SuccessFee { get; set; } = 0; // کارمزد موفقیت

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // لیست مشاوران املاکی که این شخص ثبت کرده
        public virtual ICollection<User> RegisteredAgents { get; set; } = new List<User>();
    }
}
