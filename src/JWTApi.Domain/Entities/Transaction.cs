using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JWTApi.Domain.Entities
{
    // Models/Transaction.cs
    public class Transaction
    {

        public Guid Id { get; set; }

        public Guid WalletId { get; set; }


        public string TransactionCode { get; set; } = string.Empty;


        public TransactionType Type { get; set; }

 
        public decimal Amount { get; set; }


        public decimal? Fee { get; set; }

   
        public decimal BalanceAfter { get; set; }


        public string ReferenceId { get; set; } = string.Empty;

      
        public string Description { get; set; } = string.Empty;


        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;


        public string? PaymentMethod { get; set; }


        public string? IpAddress { get; set; }

        // برای انتقال بین کاربران
        public Guid? DestinationUserId { get; set; }
        public Guid? SourceUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; } = null!;

        [ForeignKey("DestinationUserId")]
        public virtual User? DestinationUser { get; set; }

        [ForeignKey("SourceUserId")]
        public virtual User? SourceUser { get; set; }
    }
}
