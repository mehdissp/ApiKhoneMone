using JWTApi.Domain.Dtos.Payments;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Domain.Interfaces.Payments;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Infrastructure.Repositories.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Payments
{
    public class PaymentService
    {

        private IPaymentGateway _paymentGateway;
        private IUnitOfWork _unitOfWork;
        private IWalletRepository _walletRepository;
        public PaymentService(IPaymentGateway paymentGateway, IUnitOfWork unitOfWork, IWalletRepository walletRepository)
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
            _walletRepository=walletRepository;
        }
        public async Task<PaymentVerificationResult> VerifyPaymentAsync(VerificationRequest request)
        {
            return await _paymentGateway.VerifyPaymentAsync(request);
        }
        public async Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
        {
            return await _paymentGateway.RequestPaymentAsync(request);
        }

        public async Task<TransactionResult> DepostiWallet(Guid userId, decimal amount,
            string paymentMethod, string ipAddress, string refId)
        {
            return await _walletRepository.DepositAsync(userId, amount, paymentMethod, ipAddress, refId);
        }

        public async Task<WalletResult> GetBalanceAsync(Guid userId)
        {
            return await _walletRepository.GetBalanceAsync(userId);
        }


    }
}
