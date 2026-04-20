using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    // جدول مشاوران مستقل (اختیاری - اگر داده خاصی نیاز دارند)
    public class IndependentAgentProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public string? BusinessLicense { get; set; } // مجوز کسب
        public DateTime? RegisteredAt { get; set; } = DateTime.UtcNow;
        // مشاور مستقل نمیتواند کسی را ثبت کند
    }
}
