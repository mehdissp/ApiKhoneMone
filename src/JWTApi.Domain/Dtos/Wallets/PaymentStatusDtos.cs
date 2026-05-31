using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Wallets
{
    public class PaymentStatusDtos
    {
        public bool IsWalletPay { get; set; }
        public decimal? WalletBalance { get; set; }
        public decimal? AdPrice { get; set; }
        public decimal? Debtor { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
