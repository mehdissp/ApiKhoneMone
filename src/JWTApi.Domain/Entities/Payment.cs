using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public long Amount { get; set; } // به تومان
        public string Gateway { get; set; } // Zarinpal, Saman, etc.
        public string Authority { get; set; } // کد یکتای درگاه
        public string RefId { get; set; } // شماره مرجع بانک
        public PaymentStatus Status { get; set; }
        public string CallbackUrl { get; set; } // آدرس برگشت به React
        public string UserId { get; set; } // در صورت نیاز
        public DateTime CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string DescriptionRows { get; set; }
        public int? RealEstateId { get; set; }
    }
}
