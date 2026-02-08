using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    // خریدهای پکیج توسط کاربر
    public class UserPackage
    {
  
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PackageId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public Package Package { get; set; } = default!;
        public User User { get; set; } = default!;

    }
}
