using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Wallets
{
    // Result Models
    public record WalletResult(bool IsSuccess, string Message, Wallet? Data)
    {
        public static WalletResult Success(Wallet wallet) => new(true, "موفق", wallet);
        public static WalletResult Error(string message) => new(false, message, null);
    }

    public record TransactionResult(bool IsSuccess, string Message, string? TransactionCode, decimal? NewBalance)
    {
        public static TransactionResult Success(string code, decimal balance) =>
            new(true, "موفق", code, balance);
        public static TransactionResult Error(string message) =>
            new(false, message, null, null);
    }
}
