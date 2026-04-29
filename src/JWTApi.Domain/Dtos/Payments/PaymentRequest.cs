using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Payments
{
    // Models/PaymentRequest.cs
    public class PaymentRequest
    {
        public long Amount { get; set; }
        public string CallbackUrl { get; set; }
        public string Description { get; set; }
    }

    // Models/PaymentRequestResult.cs
    public class PaymentRequestResult
    {
        public bool IsSuccess { get; set; }
        public string Authority { get; set; }
        public string GatewayUrl { get; set; }
        public string ErrorMessage { get; set; }
    }

    // Models/VerificationRequest.cs
    public class VerificationRequest
    {
        public long Amount { get; set; }
        public string Authority { get; set; }
    }

    // Models/PaymentVerificationResult.cs
    public class PaymentVerificationResult
    {
        public bool IsSuccess { get; set; }
        public string RefId { get; set; }
        public string ErrorMessage { get; set; }
    }

    // Models/ApiRequests.cs
    public class InitPaymentRequest
    {
        public long Amount { get; set; }
        public string Description { get; set; }
        public string CallbackUrl { get; set; }
        public int? RealEstateId { get; set; }
    }
}
