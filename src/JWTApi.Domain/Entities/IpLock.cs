using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class IpLock
    {
        public int Id { get; set; }
        public string IPAddress { get; set; } = default!;
        public DateTime LockEnd { get; set; }   // تا چه زمانی IP قفل است
        public DateTime LockEndAt { get; set; }
        public string? Reason { get; set; }
    }
}
