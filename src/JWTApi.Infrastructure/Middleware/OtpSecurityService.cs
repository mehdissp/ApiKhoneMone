using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class OtpSecurityService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<OtpSecurityService> _logger;
    private readonly HttpClient _httpClient;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public OtpSecurityService(IMemoryCache cache, ILogger<OtpSecurityService> logger)
    {
        _cache = cache;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    public async Task<(bool allowed, string message, int waitSeconds)> CheckRateLimitAsync(string mobile, string ipAddress)
    {
        // اعتبارسنجی ورودی‌ها
        if (string.IsNullOrWhiteSpace(mobile))
            return (false, "شماره موبایل معتبر نیست", 0);

        if (string.IsNullOrWhiteSpace(ipAddress))
            return (false, "آدرس IP معتبر نیست", 0);

        // استفاده از قفل برای جلوگیری از Race Condition
        await _semaphore.WaitAsync();

        try
        {
            // 1. محدودیت بر اساس شماره موبایل (حداکثر 3 درخواست در ساعت)
            var mobileKey = $"otp_mobile_{mobile}_hour";
            var mobileCount = _cache.Get<int?>(mobileKey) ?? 0;

            if (mobileCount >= 3)
            {
                var remainingTime = GetRemainingTime(_cache, mobileKey, TimeSpan.FromHours(1));
                _logger.LogWarning("Rate limit exceeded for mobile: {Mobile}, Count: {Count}, IP: {IP}",
                    mobile, mobileCount, ipAddress);
                return (false, $"تعداد درخواست‌های این شماره بیش از حد مجاز است. لطفاً {remainingTime} دقیقه دیگر تلاش کنید", remainingTime * 60);
            }

            // 2. محدودیت بر اساس IP (حداکثر 10 درخواست در ساعت)
            var ipKey = $"otp_ip_{ipAddress}_hour";
            var ipCount = _cache.Get<int?>(ipKey) ?? 0;

            if (ipCount >= 10)
            {
                var remainingTime = GetRemainingTime(_cache, ipKey, TimeSpan.FromHours(1));
                _logger.LogWarning("Rate limit exceeded for IP: {IP}, Count: {Count}, Mobile: {Mobile}",
                    ipAddress, ipCount, mobile);
                return (false, $"تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً {remainingTime} دقیقه دیگر تلاش کنید", remainingTime * 60);
            }

            // 3. محدودیت ترکیبی (شماره موبایل + IP) برای تشخیص حملات هدفمند
            var combinedKey = $"otp_{mobile}_{ipAddress}_5min";
            var combinedCount = _cache.Get<int?>(combinedKey) ?? 0;

            if (combinedCount >= 2) // حداکثر 2 درخواست در 5 دقیقه
            {
                _cache.Set(mobileKey, mobileCount + 1, TimeSpan.FromHours(1));
                var remainingTime = GetRemainingTime(_cache, combinedKey, TimeSpan.FromMinutes(5));
                _logger.LogWarning("Combined rate limit exceeded for Mobile: {Mobile}, IP: {IP}", mobile, ipAddress);
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
                return (false, $"لطفاً {remainingTime} دقیقه دیگر تلاش کنید", remainingTime * 60);
            }

            // 4. محدودیت روزانه (حداکثر 10 درخواست در روز برای هر شماره)
            var dailyKey = $"otp_mobile_{mobile}_day";
            var dailyCount = _cache.Get<int?>(dailyKey) ?? 0;

            if (dailyCount >= 10)
            {
                _cache.Set(mobileKey, mobileCount + 1, TimeSpan.FromHours(1));
                var remainingTime = GetRemainingTime(_cache, dailyKey, TimeSpan.FromDays(1));
                _logger.LogWarning("Daily limit exceeded for mobile: {Mobile}, Count: {Count}", mobile, dailyCount);
                return (false, $"تعداد درخواست‌های روزانه شما به حد مجاز رسیده است. لطفاً فردا تلاش کنید", remainingTime * 3600);
            }

            // افزایش شمارنده‌ها
            _cache.Set(mobileKey, mobileCount + 1, TimeSpan.FromHours(1));
            _cache.Set(ipKey, ipCount + 1, TimeSpan.FromHours(1));
            _cache.Set(combinedKey, combinedCount + 1, TimeSpan.FromMinutes(5));
            _cache.Set(dailyKey, dailyCount + 1, TimeSpan.FromDays(1));

            _logger.LogInformation("OTP request allowed for Mobile: {Mobile}, IP: {IP}", mobile, ipAddress);

            return (true, "مجاز", 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckRateLimitAsync for Mobile: {Mobile}, IP: {IP}", mobile, ipAddress);
            return (false, "خطا در بررسی محدودیت‌ها. لطفاً چند دقیقه دیگر تلاش کنید", 60);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // متد برای دریافت زمان باقیمانده
    private int GetRemainingTime(IMemoryCache cache, string key, TimeSpan defaultDuration)
    {
        if (cache.TryGetValue(key, out _))
        {
            // این متد زمان دقیق باقیمانده را برمی‌گرداند
            // در پیاده‌سازی واقعی باید از متدهای پیشرفته‌تر استفاده کنید
            return (int)defaultDuration.TotalMinutes;
        }
        return (int)defaultDuration.TotalMinutes;
    }

    // متد برای پاک کردن محدودیت‌های یک شماره موبایل (مثلاً بعد از احراز هویت موفق)
    public async Task ResetRateLimitAsync(string mobile)
    {
        await _semaphore.WaitAsync();
        try
        {
            var patterns = new[]
            {
                $"otp_mobile_{mobile}_hour",
                $"otp_mobile_{mobile}_day"
            };

            foreach (var pattern in patterns)
            {
                _cache.Remove(pattern);
            }

            _logger.LogInformation("Rate limit reset for mobile: {Mobile}", mobile);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // متد برای دریافت وضعیت فعلی محدودیت‌ها
    public async Task<RateLimitStatus> GetRateLimitStatusAsync(string mobile, string ipAddress)
    {
        await _semaphore.WaitAsync();
        try
        {
            var mobileHourKey = $"otp_mobile_{mobile}_hour";
            var mobileDayKey = $"otp_mobile_{mobile}_day";
            var ipKey = $"otp_ip_{ipAddress}_hour";
            var combinedKey = $"otp_{mobile}_{ipAddress}_5min";

            return new RateLimitStatus
            {
                MobileHourRequests = _cache.Get<int?>(mobileHourKey) ?? 0,
                MobileDayRequests = _cache.Get<int?>(mobileDayKey) ?? 0,
                IpHourRequests = _cache.Get<int?>(ipKey) ?? 0,
                Combined5MinRequests = _cache.Get<int?>(combinedKey) ?? 0,
                MaxMobileHourRequests = 3,
                MaxMobileDayRequests = 10,
                MaxIpHourRequests = 10,
                MaxCombined5MinRequests = 2
            };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _semaphore?.Dispose();
    }
}

// کلاس وضعیت محدودیت‌ها
public class RateLimitStatus
{
    public int MobileHourRequests { get; set; }
    public int MobileDayRequests { get; set; }
    public int IpHourRequests { get; set; }
    public int Combined5MinRequests { get; set; }
    public int MaxMobileHourRequests { get; set; }
    public int MaxMobileDayRequests { get; set; }
    public int MaxIpHourRequests { get; set; }
    public int MaxCombined5MinRequests { get; set; }

    public bool IsMobileHourLimited => MobileHourRequests >= MaxMobileHourRequests;
    public bool IsMobileDayLimited => MobileDayRequests >= MaxMobileDayRequests;
    public bool IsIpLimited => IpHourRequests >= MaxIpHourRequests;
    public bool IsCombinedLimited => Combined5MinRequests >= MaxCombined5MinRequests;
}