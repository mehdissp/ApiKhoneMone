using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Wallets
{
    public class TransactionDtos
    {
        public Guid Id { get; set; }



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
        public string RefIPG { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
