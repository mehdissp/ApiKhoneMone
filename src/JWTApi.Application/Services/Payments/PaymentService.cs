using JWTApi.Domain.Dtos.Payments;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Domain.Interfaces.Payments;
using JWTApi.Domain.Interfaces.RealEstatesApplications;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Domain.Shared;
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
        private readonly IRealEstatesRepository _realEstatesRepository;
        private readonly IRealEstatesApplicationsRepository _realEstatesApplicationsRepository;
        public PaymentService(IPaymentGateway paymentGateway, IUnitOfWork unitOfWork,
            IWalletRepository walletRepository,
            IRealEstatesRepository realEstatesRepository, IRealEstatesApplicationsRepository realEstatesApplicationsRepository
            )
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
            _walletRepository = walletRepository;
            _realEstatesRepository = realEstatesRepository;
            _realEstatesApplicationsRepository = realEstatesApplicationsRepository;
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

        public async Task<TransactionResult> WithdrawWalletForAd(string userId, AdPriceRangeType adPriceRangeType, int id, string roleId
    , string ipAddress, CancellationToken cancellationToken)
        {
            PaymentStatusDtos balance = new PaymentStatusDtos();
            if (AdPriceRangeType.InsertAd == adPriceRangeType)
            {
                balance = await _realEstatesRepository.GetPaymentStatus(id, Guid.Parse(roleId), Guid.Parse(userId));
            }
            else if (AdPriceRangeType.ShowApplicantRequest == adPriceRangeType)
            {
                balance = await _realEstatesApplicationsRepository.GetPaymentStatus(id, Guid.Parse(roleId), Guid.Parse(userId), cancellationToken);

            }
            return await _walletRepository.WithdrawAsync(Guid.Parse(userId), (decimal)balance.AdPrice, ipAddress);
        }

        public async Task AccessToShowMobileNumber(int realEstateIdAppId, string roleId, string userId, CancellationToken cancellationToken)
        {
            await _realEstatesApplicationsRepository.AccessToShowMobileNumber(realEstateIdAppId, Guid.Parse(roleId), Guid.Parse(userId), cancellationToken);
        }


    }
}
