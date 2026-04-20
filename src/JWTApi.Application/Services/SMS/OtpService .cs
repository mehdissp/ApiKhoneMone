using JWTApi.Domain.Dtos.OTP;
using JWTApi.Domain.Interfaces.SMS;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.SMS
{


    public class OtpService 
    {
        private readonly ISmsService _smsService;
        private readonly IMemoryCache _cache;
        private readonly Random _random = new Random();

        public OtpService(ISmsService smsService, IMemoryCache cache)
        {
            _smsService = smsService;
            _cache = cache;
        }

        public string GenerateOtpCode()
        {
            // تولید کد 4 یا 5 یا 6 رقمی
            return _random.Next(1000, 9999).ToString(); // کد 4 رقمی
                                                        // return _random.Next(10000, 99999).ToString(); // کد 5 رقمی
                                                        // return _random.Next(100000, 999999).ToString(); // کد 6 رقمی
        }

        //public async Task<bool> SendOtpAsync(string mobile)
        //{
        //    var code = GenerateOtpCode();
        //    var expiryTime = DateTime.Now.AddMinutes(5); // کد به مدت 5 دقیقه معتبر است

        //    var otpRequest = new OtpRequestModel
        //    {
        //        Mobile = mobile,
        //        Code = code,
        //        ExpiryTime = expiryTime
        //    };

        //    // ذخیره در کش
        //    _cache.Set($"otp_{mobile}", otpRequest, TimeSpan.FromMinutes(5));

        //    // ارسال پیامک
        //    var result = await _smsService.SendVerificationCodeAsync(mobile, code);

        //    return result.IsSuccess;
        //}
        public async Task<(bool success, string message, int remainingSeconds)> SendOtpAsync(string mobile)
        {
            try
            {
                // 1. اعتبارسنجی شماره موبایل
                if (string.IsNullOrWhiteSpace(mobile))
                    return (false, "شماره موبایل معتبر نیست", 0);

                // 2. بررسی قفل بودن کاربر
                var lockKey = $"otp_lock_{mobile}";
                if (_cache.TryGetValue(lockKey, out int lockMinutes))
                {
                    return (false, $"به دلیل تلاش‌های ناموفق مکرر، لطفاً {lockMinutes} دقیقه دیگر تلاش کنید", lockMinutes * 60);
                }

                // 3. بررسی محدودیت زمانی بین درخواست‌ها
                var lastRequestKey = $"otp_last_request_{mobile}";
                if (_cache.TryGetValue(lastRequestKey, out DateTime lastRequestTime))
                {
                    var timeSinceLastRequest = DateTime.Now - lastRequestTime;
                    if (timeSinceLastRequest.TotalMinutes < 5)
                    {
                        var remainingSeconds = (int)(300 - timeSinceLastRequest.TotalSeconds);
                        var remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0); // ✅ استفاده از 60.0
                        return (false, $"لطفاً {remainingMinutes} دقیقه دیگر تلاش کنید", remainingSeconds);
                    }
                }

                // 4. بررسی تعداد درخواست‌های اخیر
                var requestCountKey = $"otp_request_count_{mobile}";
                var requestCount = _cache.Get<int?>(requestCountKey) ?? 0;

                if (requestCount >= 3)
                {
                    return (false, "شما بیش از حد مجاز درخواست داده‌اید. لطفاً 10 دقیقه دیگر تلاش کنید", 600);
                }

                // 5. تولید کد OTP
                var code = GenerateOtpCode();
                var expiryTime = DateTime.Now.AddMinutes(5);

                // 6. ذخیره در کش
                var otpKey = $"otp_{mobile}";
                var otpRequest = new OtpRequestModel
                {
                    Mobile = mobile,
                    Code = code,
                    ExpiryTime = expiryTime,
                    AttemptCount = 0
                };

                _cache.Set(otpKey, otpRequest, TimeSpan.FromMinutes(5));
                _cache.Set(lastRequestKey, DateTime.Now, TimeSpan.FromMinutes(5));
                _cache.Set(requestCountKey, requestCount + 1, TimeSpan.FromMinutes(10));

                // 7. ارسال پیامک
                var smsResult = await _smsService.SendVerificationCodeAsync(mobile, code);

                if (smsResult.IsSuccess)
                {
                    return (true, "کد تایید با موفقیت ارسال شد", 300);
                }
                else
                {
                    _cache.Remove(otpKey);
                    _cache.Remove(lastRequestKey);
                    return (false, "خطا در ارسال پیامک. لطفاً مجدداً تلاش کنید", 0);
                }
            }
            catch (Exception ex)
            {
            
                return (false, "خطای سیستمی. لطفاً چند دقیقه دیگر تلاش کنید", 0);
            }
        }



        public async Task<bool> VerifyOtpAsync(string mobile, string code)
        {
            if (_cache.TryGetValue($"otp_{mobile}", out OtpRequestModel otpRequest))
            {
                if (otpRequest.Code == code && otpRequest.ExpiryTime > DateTime.Now)
                {
                    // حذف کد پس از استفاده موفق
                    _cache.Remove($"otp_{mobile}");
                    return true;
                }
            }

            return false;
        }
    }
}
