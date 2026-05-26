using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Wallets
{
    public interface IWalletRepository
    {
        Task<WalletResult> CreateWalletIndepentAsync(Guid userId);
        Task<WalletResult> GetBalanceAsync(Guid userId);
        Task<TransactionResult> DepositAsync(Guid userId, decimal amount, string paymentMethod, string ipAddress, string refId);
        Task<TransactionResult> WithdrawAsync(Guid userId, decimal amount, string ipAddress);
        Task<TransactionResult> TransferAsync(Guid fromUserId, Guid toUserId, decimal amount, string ipAddress);
        Task<List<TransactionDtos>> GetTransactionHistoryAsync(Guid userId, CancellationToken cancellationToken,
            int page = 1, int pageSize = 20);
    }
}
