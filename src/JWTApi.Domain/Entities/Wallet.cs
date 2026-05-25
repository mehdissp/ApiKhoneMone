using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JWTApi.Domain.Entities
{
    // Models/Wallet.cs


    public class Wallet
    {
    
        public Guid Id { get; set; } 


        public Guid UserId { get; set; }  // تغییر از int به Guid

        public decimal Balance { get; set; } = 0;


        public decimal PendingBalance { get; set; } = 0;

     
        public string Currency { get; set; } = "IRT";

        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;

        // محدودیت‌های تراکنش

        public decimal? DailyTransactionLimit { get; set; } = 100_000_000; // 100 میلیون تومان

 
        public decimal? PerTransactionLimit { get; set; } = 50_000_000; // 50 میلیون تومان

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastTransactionAt { get; set; }
        public DateTime? LastDailyReset { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<WalletLog> Logs { get; set; } = new List<WalletLog>();
    }
}
