using JWTApi.Domain.Dtos.Payments;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Domain.Interfaces.Payments;
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
        public PaymentService(IPaymentGateway paymentGateway, IUnitOfWork unitOfWork)
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaymentVerificationResult> VerifyPaymentAsync(VerificationRequest request)
        {
            return await _paymentGateway.VerifyPaymentAsync(request);
        }
        public async Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
        {
            return await _paymentGateway.RequestPaymentAsync(request);
        }
    }
}
