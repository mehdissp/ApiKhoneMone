using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Wallets
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WalletRepository> _logger;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public WalletRepository(AppDbContext context, ILogger<WalletRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WalletResult> CreateWalletIndepentAsync(Guid userId)
        {
            try
            {
                var getRole = await _context.UserRoles.Where(s => s.UserId == userId).FirstOrDefaultAsync();
                int balance = 0;
                if (getRole.RoleId.ToString() == "2C0C91C9-AF94-4D93-8875-D3BE8FE7F73C")
                {
                    balance = 150000;
                }
                if (getRole.RoleId.ToString() == "0B0FFF66-D17E-4958-BB1C-AE38A9EB7270")
                {
                    balance = 550000;
                }
                // چک کردن وجود کیف پول
                var existing = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (existing != null)
                    return WalletResult.Error("کیف پول قبلاً ایجاد شده است");
            
                var wallet = new Wallet
                {
                    UserId = userId,
                    Balance = balance,
                    PendingBalance = 0,
                    Currency = "IRI",
                    IsActive = true
                };

                await _context.Wallets.AddAsync(wallet);
                await _context.SaveChangesAsync();

                // ثبت لاگ
                await LogActionAsync(wallet.Id, "CREATE_WALLET", null,
                    $"کیف پول برای کاربر {userId} ایجاد شد", "SYSTEM");

                return WalletResult.Success(wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد کیف پول برای کاربر {UserId}", userId);
                return WalletResult.Error("خطا در ایجاد کیف پول");
            }
        }

        public async Task<TransactionResult> DepositAsync(Guid userId, decimal amount,
            string paymentMethod, string ipAddress,string refId)
        {
            if (amount <= 0)
                return TransactionResult.Error("مبلغ باید بیشتر از صفر باشد");

            await _lock.WaitAsync();

            try
            {
                await using var transaction = await _context.Database
              .BeginTransactionAsync(IsolationLevel.Serializable);

                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                    return TransactionResult.Error("کیف پول یافت نشد");

                if (!wallet.IsActive || wallet.IsLocked)
                    return TransactionResult.Error("کیف پول غیرفعال یا قفل شده است");

                // استفاده از Row Lock برای امنیت
                wallet = await _context.Wallets
                    .FromSqlRaw("SELECT * FROM Wallets WHERE Id = {0} ", wallet.Id)
                    .FirstOrDefaultAsync();

                var oldBalance = wallet.Balance;
                wallet.Balance += amount;
                wallet.LastTransactionAt = DateTime.UtcNow;

                // ایجاد تراکنش
                var transactionCode = GenerateTransactionCode();
                var newTransaction = new Transaction
                {
                    WalletId = wallet.Id,
                    TransactionCode = transactionCode,
                    Type = TransactionType.Deposit,
                    Amount = amount,
                    BalanceAfter = wallet.Balance,
                    ReferenceId = GenerateReferenceId(),
                    RefIPG=refId,
                    Description = $"واریز به مبلغ {amount:N0} ریال",
                    Status = TransactionStatus.Completed,
                    PaymentMethod = paymentMethod,
                    IpAddress = ipAddress,
                    CompletedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(newTransaction);
                await _context.SaveChangesAsync();

                // ثبت لاگ
                await LogActionAsync(wallet.Id, "DEPOSIT", oldBalance.ToString(),
                    wallet.Balance.ToString(), ipAddress);

                await transaction.CommitAsync();

                return TransactionResult.Success(transactionCode, wallet.Balance);
            }
            catch (DbUpdateConcurrencyException)
            {
                return TransactionResult.Error("خطای همزمانی، لطفاً مجدداً تلاش کنید");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در واریز وجه برای کاربر {UserId}", userId);
                return TransactionResult.Error("خطا در انجام تراکنش");
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<TransactionResult> WithdrawAsync(Guid userId, decimal amount, string ipAddress)
        {
            if (amount <= 0)
                return TransactionResult.Error("مبلغ باید بیشتر از صفر باشد");

            await _lock.WaitAsync();

            try
            {
                await using var transaction = await _context.Database
    .BeginTransactionAsync(IsolationLevel.Serializable);

                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                    return TransactionResult.Error("کیف پول یافت نشد");

                if (!wallet.IsActive || wallet.IsLocked)
                    return TransactionResult.Error("کیف پول غیرفعال است");

                if (wallet.Balance < amount)
                    return TransactionResult.Error("موجودی کافی نیست");

                // قفل رکورد
                wallet = await _context.Wallets
                    .FromSqlRaw("SELECT * FROM Wallets WHERE Id = {0}", wallet.Id)
                    .FirstOrDefaultAsync();

                var oldBalance = wallet.Balance;
                wallet.Balance -= amount;
                wallet.LastTransactionAt = DateTime.UtcNow;

                var transactionCode = GenerateTransactionCode();
                var newTransaction = new Transaction
                {
                    WalletId = wallet.Id,
                    TransactionCode = transactionCode,
                    Type = TransactionType.Withdraw,
                    Amount = amount,
                    BalanceAfter = wallet.Balance,
                    ReferenceId = GenerateReferenceId(),
                    Description = $"برداشت به مبلغ {amount:N0} ریال",
                    RefIPG="",
                    Status = TransactionStatus.Completed,
                    IpAddress = ipAddress,
                    CompletedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(newTransaction);
                await _context.SaveChangesAsync();

                await LogActionAsync(wallet.Id, "WITHDRAW", oldBalance.ToString(),
                    wallet.Balance.ToString(), ipAddress);

                await transaction.CommitAsync();

                return TransactionResult.Success(transactionCode, wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در برداشت وجه");
                return TransactionResult.Error("خطا در انجام تراکنش");
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<TransactionResult> TransferAsync(Guid fromUserId, Guid toUserId,
            decimal amount, string ipAddress)
        {
            if (fromUserId == toUserId)
                return TransactionResult.Error("امکان انتقال به خودتان نیست");

            if (amount <= 0)
                return TransactionResult.Error("مبلغ باید بیشتر از صفر باشد");

            await _lock.WaitAsync();

            try
            {
                await using var transaction = await _context.Database
        .BeginTransactionAsync(IsolationLevel.Serializable);

                var fromWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == fromUserId);

                var toWallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == toUserId);

                if (fromWallet == null || toWallet == null)
                    return TransactionResult.Error("کیف پول یافت نشد");

                if (fromWallet.Balance < amount)
                    return TransactionResult.Error("موجودی کافی نیست");

                // قفل هر دو رکورد
                fromWallet = await _context.Wallets
                    .FromSqlRaw("SELECT * FROM Wallets WHERE WalletId = {0} FOR UPDATE", fromWallet.Id)
                    .FirstOrDefaultAsync();

                toWallet = await _context.Wallets
                    .FromSqlRaw("SELECT * FROM Wallets WHERE WalletId = {0} FOR UPDATE", toWallet.Id)
                    .FirstOrDefaultAsync();

                var transactionCode = GenerateTransactionCode();
                var referenceId = GenerateReferenceId();

                // کم کردن از فرستنده
                fromWallet.Balance -= amount;
                fromWallet.LastTransactionAt = DateTime.UtcNow;

                var sendTransaction = new Transaction
                {
                    WalletId = fromWallet.Id,
                    TransactionCode = transactionCode,
                    Type = TransactionType.Transfer,
                    Amount = -amount,
                    BalanceAfter = fromWallet.Balance,
                    ReferenceId = referenceId,
                    Description = $"انتقال به کاربر {toUserId}",
                    Status = TransactionStatus.Completed,
                    IpAddress = ipAddress,
                    CompletedAt = DateTime.UtcNow
                };

                // اضافه کردن به گیرنده
                toWallet.Balance += amount;
                toWallet.LastTransactionAt = DateTime.UtcNow;

                var receiveTransaction = new Transaction
                {
                    WalletId = toWallet.Id,
                    TransactionCode = transactionCode,
                    Type = TransactionType.Transfer,
                    Amount = amount,
                    BalanceAfter = toWallet.Balance,
                    ReferenceId = referenceId,
                    Description = $"دریافت از کاربر {fromUserId}",
                    Status = TransactionStatus.Completed,
                    IpAddress = ipAddress,
                    CompletedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddRangeAsync(sendTransaction, receiveTransaction);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return TransactionResult.Success(transactionCode, fromWallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در انتقال وجه");
                return TransactionResult.Error("خطا در انجام انتقال");
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<WalletResult> GetBalanceAsync(Guid userId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return WalletResult.Error("کیف پول یافت نشد");

            return WalletResult.Success(wallet);
        }

        public async Task<List<TransactionDtos>> GetTransactionHistoryAsync(Guid userId, CancellationToken cancellationToken,
            int page = 1, int pageSize = 20)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return new List<TransactionDtos>();

            return await _context.Transactions
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * 20)
                .Take(20)
                        .Select(t => new TransactionDtos
                        {
                            Id = t.Id,
                            TransactionCode = t.TransactionCode,
                            Type = t.Type,
                            Amount = t.Amount,
                            Fee = t.Fee,
                            BalanceAfter = t.BalanceAfter,
                            ReferenceId = t.ReferenceId,
                            Description = t.Description,
                            Status = t.Status,
                            PaymentMethod = t.PaymentMethod,
                            IpAddress = t.IpAddress,
                            RefIPG = t.RefIPG,
                            CreatedAt = t.CreatedAt,
                            CompletedAt = t.CompletedAt,
                            // اگر نیاز به اطلاعات کیف پول دارید
                            // WalletName = t.Wallet.Name,
                            // UserFullName = t.Wallet.User.FullName
                        })
        .ToListAsync(cancellationToken);
        }

        // متدهای کمکی
        private string GenerateTransactionCode()
        {
            return $"TRX-{DateTime.UtcNow:yyyyMMddHHmmss}-{RandomNumberGenerator.GetInt32(100000, 999999)}";
        }

        private string GenerateReferenceId()
        {
            return $"{DateTime.UtcNow:yyyyMMddHHmmss}{RandomNumberGenerator.GetInt32(100000, 999999)}";
        }

        private async Task LogActionAsync(Guid walletId, string action, string? oldValue,
            string? newValue, string ipAddress)
        {
            var log = new WalletLog
            {
                WalletId = walletId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            await _context.WalletLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
