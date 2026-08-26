//using JWTApi.Domain.Entities;
//using JWTApi.Domain.Interfaces.Stories;
//using JWTApi.Domain.Interfaces.Verifications;
//using JWTApi.Domain.Models;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace JWTApi.Application.Services.VerificationService
//{
//    public class VerificationService
//    {
//        private readonly IVerificationRepository _verificationRepository;
//        public VerificationService(IVerificationRepository verificationRepository)
//        {
//            _verificationRepository = verificationRepository;
//        }
//        public async Task<bool> ShahkarLite(string mobileNumber, string codeMeli, string userId)
//        {
//            var verfication = await _verificationRepository.GetVerificationShahkar(codeMeli, mobileNumber);
//            if (verfication == null)
//            {
//                var client = new RestClient("https://s.api.ir/api/sw1/ShahkarLite");
//                var request = new RestRequest("", Method.Post);
//                request.AddHeader("Content-Type", "application/json");
//                request.AddHeader("Authorization", "Bearer 39vAZGXOxrbOwQcRLBud+PHMgn0rWpKaU4UUI6a5LuczGJabyoioesMdVBzDsiPK34a51rFUdQIWdpxlCFrzc8IFgR/AU/bGPu7Kqx4TRKo="); // توکن خود را جایگزین کنید

//                // استفاده از داینامیک برای مقادیر ورودی
//                string jsonBody = $@"
//                {{
//                    ""nationalCode"": ""{codeMeli}"",
//                    ""mobile"": ""{mobileNumber}""
//                }}";

//                request.AddStringBody(jsonBody, ContentType.Json);

//                var response = await client.ExecuteAsync(request);

//                // بررسی موفقیت آمیز بودن درخواست
//                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
//                {
//                    try
//                    {
//                        // دسریالایز کردن پاسخ
//                        var result = System.Text.Json.JsonSerializer.Deserialize<ShahkarResponse>(response.Content);
//                        VerificationShahkar verificationShahkar = new VerificationShahkar();
//                        verificationShahkar.create(userId, mobileNumber, codeMeli, result.Data, 500, 500);
//                        await _verificationRepository.create(verificationShahkar);
//                        // بر اساس ساختار پاسخ شما:
//                        // data: true/false (نتیجه اصلی)
//                        // success: true/false (موفقیت درخواست)
//                        return result?.Data ?? false;
//                    }
//                    catch (Exception ex)
//                    {
//                        // خطا در پردازش پاسخ
//                        Console.WriteLine($"Error parsing response: {ex.Message}");
//                        return false;
//                    }
//                }

//                return false;
//            }
//            return true;


//        }

//        // مدل پاسخ بر اساس خروجی API
//        public class ShahkarResponse
//        {
//            public bool Data { get; set; }      // نتیجه اصلی (true/false)
//            public bool Success { get; set; }   // موفقیت درخواست
//            public int Code { get; set; }       // کد وضعیت
//            public string Message { get; set; } // پیام خطا (در صورت وجود)
//        }
//    }
//}

using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Stories;
using JWTApi.Domain.Interfaces.Verifications;
using JWTApi.Domain.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.VerificationService
{
    public class VerificationService
    {
        private readonly IVerificationRepository _verificationRepository;

        public VerificationService(IVerificationRepository verificationRepositorys)
        {
            _verificationRepository = verificationRepositorys;
        }

        public async Task<bool> ShahkarLite(string mobileNumber, string codeMeli, string userId)
        {
            try
            {
                // بررسی وجود رکورد قبلی
                var verfication = await _verificationRepository.GetVerificationShahkar(codeMeli, mobileNumber);

                if (verfication.Status ==true)
                {
                    return true; // قبلاً ثبت شده
                }

                // ایجاد درخواست به API
                var client = new RestClient("https://s.api.ir/api/sw1/ShahkarLite");
                var request = new RestRequest("", Method.Post);

                // هدرها
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer 39vAZGXOxrbOwQcRLBud+PHMgn0rWpKaU4UUI6a5LuczGJabyoioesMdVBzDsiPK34a51rFUdQIWdpxlCFrzc8IFgR/AU/bGPu7Kqx4TRKo=");

                // بدنه درخواست
                string jsonBody = $@"
                {{
                    ""nationalCode"": ""{codeMeli}"",
                    ""mobile"": ""{mobileNumber}""
                }}";

                request.AddStringBody(jsonBody, ContentType.Json);

                // اجرای درخواست
                var response = await client.ExecuteAsync(request);

                // بررسی پاسخ
                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    Console.WriteLine($"Request failed: {response.StatusCode}");
                    return false;
                }

                // پردازش پاسخ با JsonDocument (راه حل چهارم)
                bool resultData = false;
                bool resultSuccess = false;
                int resultCode = 0;
                string resultMessage = string.Empty;

                try
                {
                    using var document = JsonDocument.Parse(response.Content);
                    var root = document.RootElement;

                    // استخراج داده‌ها با TryGetProperty برای جلوگیری از خطا
                    if (root.TryGetProperty("data", out var dataElement))
                    {
                        resultData = dataElement.GetBoolean();
                    }

                    if (root.TryGetProperty("success", out var successElement))
                    {
                        resultSuccess = successElement.GetBoolean();
                    }

                    if (root.TryGetProperty("code", out var codeElement))
                    {
                        resultCode = codeElement.GetInt32();
                    }

                    if (root.TryGetProperty("message", out var messageElement))
                    {
                        resultMessage = messageElement.GetString() ?? string.Empty;
                    }

                    // لاگ برای دیباگ
                    Console.WriteLine($"Data: {resultData}, Success: {resultSuccess}, Code: {resultCode}, Message: {resultMessage}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                    Console.WriteLine($"Response Content: {response.Content}");
                    return false;
                }

                // تبدیل userId به Guid
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    Console.WriteLine($"Invalid userId format: {userId}");
                    return false;
                }

                // ایجاد و ذخیره رکورد
                VerificationShahkar verificationShahkar = new VerificationShahkar();
                verificationShahkar.create(userId, mobileNumber, codeMeli, resultData, 500, 500);
                await _verificationRepository.create(verificationShahkar);
            

                // برگرداندن نتیجه
                return resultData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShahkarLite: {ex.Message}");
                return false;
            }
        }
    }

    // مدل پاسخ (اختیاری - اگر خواستید استفاده کنید)
    public class ShahkarResponse
    {
        public bool Data { get; set; }
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
