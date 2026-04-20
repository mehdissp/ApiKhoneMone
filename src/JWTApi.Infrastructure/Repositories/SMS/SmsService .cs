using JWTApi.Domain.Dtos.OTP;
using JWTApi.Domain.Interfaces.SMS;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.SMS;


public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IMemoryCache _cache;

    public SmsService(IConfiguration configuration, IMemoryCache cache)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["SmsApi:ApiKey"] ?? "Uj15tYUk2WRUgR7p1gd5i7q8G0P2QutrQ1D1QjsE4hgC8OmQ";
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
    }

    public async Task<SmsSendResult> SendVerificationCodeAsync(string mobile, string code, int templateId = 783197)
    {
        try
        {
            var model = new
            {
                Mobile = mobile,
                TemplateId = 123456,
                Parameters = new[]
                {
                    new { Name = "CODE", Value = code }
                }
            };

            string payload = JsonSerializer.Serialize(model);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.sms.ir/v1/send/verify", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new SmsSendResult
                {
                    IsSuccess = true,
                    Message = "کد تایید با موفقیت ارسال شد",
                    SmsId = DateTime.Now.Ticks
                };
            }

            return new SmsSendResult
            {
                IsSuccess = false,
                Message = $"خطا در ارسال پیامک: {responseBody}"
            };
        }
        catch (Exception ex)
        {
            return new SmsSendResult
            {
                IsSuccess = false,
                Message = $"خطا: {ex.Message}"
            };
        }

    }
}
