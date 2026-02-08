using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }      // اگر username وجود داشت
        public string? Username { get; set; }  // برای گزارش
        public string IPAddress { get; set; } = default!;
        public DateTime AttemptTime { get; set; } = DateTime.Now;
        public bool Success { get; set; }
        public string? Reason { get; set; }
    }
}
