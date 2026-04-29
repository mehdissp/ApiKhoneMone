using JWTApi.Domain.Dtos.Payments;
using JWTApi.Domain.Interfaces.Payments;
using JWTApi.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Payments
{
    // IPaymentGateway.cs

 

    public class ZarinpalGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _merchantId;
        private readonly string _baseUrl;
        private readonly string _paymentUrl;
        private readonly AppDbContext _context;

        public ZarinpalGateway(IConfiguration config, AppDbContext appDbContext)
        {
            _merchantId = config["Zarinpal:MerchantId"];
            _baseUrl = config["Zarinpal:BaseUrl"];
            _paymentUrl = config["Zarinpal:PaymentUrl"];
            _httpClient = new HttpClient();
            _context= appDbContext;
        }
         
        public async Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
        {
            var payload = new
            {
                merchant_id = _merchantId,
                amount = request.Amount,
                callback_url = request.CallbackUrl,
                description = request.Description ?? "پرداخت کاربر"
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}request.json", payload);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ZarinpalRequestResponse>(responseContent);

                return new PaymentRequestResult
                {
                    IsSuccess = result?.data?.code == 100,
                    Authority = result?.data?.authority,
                    GatewayUrl = result?.data?.code == 100
                        ? $"{_paymentUrl}{result?.data?.authority}"
                        : null,
                    ErrorMessage = result?.data?.code != 100 ? $"کد خطا: {result?.data?.code}" : null
                };
            }
            catch (Exception ex)
            {
                return new PaymentRequestResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PaymentVerificationResult> VerifyPaymentAsync(VerificationRequest request)
        {
            var payload = new
            {
                merchant_id = _merchantId,
                amount = request.Amount,
                authority = request.Authority
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}verify.json", payload);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ZarinpalVerifyResponse>(responseContent);

                return new PaymentVerificationResult
                {
                    IsSuccess = result?.data?.code == 100,
                    RefId = result?.data?.ref_id.ToString(),
                    ErrorMessage = result?.data?.code != 100 ? $"کد خطا: {result?.data?.code}" : null
                };
            }
            catch (Exception ex)
            {
                return new PaymentVerificationResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
