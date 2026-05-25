using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class WalletLog
    {

        public int Id { get; set; }


        public Guid WalletId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

  
        public string IpAddress { get; set; } = string.Empty;

     
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; } = null!;
    }
}
