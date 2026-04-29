using JWTApi.Domain.Dtos.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Payments
{
    // IPaymentGateway.cs
    public interface IPaymentGateway
    {
        Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request);
        Task<PaymentVerificationResult> VerifyPaymentAsync(VerificationRequest request);
    }
}
