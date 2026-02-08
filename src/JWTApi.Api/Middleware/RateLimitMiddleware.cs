//using System.Collections.Concurrent;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using System;

//namespace JwtApi.Api.Middleware
//{
//    /// <summary>
//    /// Middleware برای محدودسازی تعداد درخواست‌ها (Rate Limiting)
//    /// هر IP برای هر مسیر فقط می‌تونه تعداد مشخصی درخواست در بازه زمانی مشخص بفرسته.
//    /// </summary>
//    ///     public static class RateLimitMiddlewareExtensions



//    public class RateLimitMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<RateLimitMiddleware> _logger;

//        // دیکشنری که (IP + Path) -> لیست زمان درخواست‌ها رو نگه می‌داره
//        private static readonly ConcurrentDictionary<string, ConcurrentQueue<long>> _requestLog = new();

//        // تنظیمات محدودسازی
//        private const int MaxRequests = 10; // حداکثر درخواست مجاز
//        private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(30); // بازه زمانی مجاز

//        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
//            var path = context.Request.Path.ToString().ToLowerInvariant();

//            var key = $"{ip}:{path}"; // کلید یکتا برای IP + مسیر

//            if (IsRateLimitExceeded(key))
//            {
//                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//                context.Response.Headers["Retry-After"] = TimeWindow.TotalSeconds.ToString();
//                await context.Response.WriteAsync($"Too many requests. Please wait {TimeWindow.TotalSeconds} seconds.");
//                _logger.LogWarning("Rate limit exceeded for {Ip} on {Path}", ip, path);
//                return;
//            }

//            await _next(context);
//        }

//        private bool IsRateLimitExceeded(string key)
//        {
//            var now = DateTime.UtcNow.Ticks;
//            var queue = _requestLog.GetOrAdd(key, _ => new ConcurrentQueue<long>());

//            // اضافه کردن زمان فعلی درخواست
//            queue.Enqueue(now);

//            // حذف درخواست‌هایی که از بازه زمانی 30 ثانیه گذشته‌اند
//            var windowTicks = TimeWindow.Ticks;
//            while (queue.TryPeek(out var oldest) && (now - oldest) > windowTicks)
//            {
//                queue.TryDequeue(out _);
//            }

//            // بررسی اینکه آیا تعداد درخواست‌ها بیش از حد مجاز شده
//            return queue.Count > MaxRequests;
//        }
//    }

//    /// <summary>
//    /// متد اکستنشن برای فعال‌سازی Middleware در pipeline
//    /// </summary>
//    //public static class RateLimitMiddlewareExtensions
//    //{
//    //    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder app)
//    //    {
//    //        return app.UseMiddleware<RateLimitMiddleware>();
//    //    }
//    //}
//}


//using System.Collections.Concurrent;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using System;

//namespace JwtApi.Api.Middleware
//{
//    /// <summary>
//    /// Middleware برای محدودسازی تعداد درخواست‌ها (Rate Limiting)
//    /// هر IP برای هر مسیر (URL) فقط می‌تونه تعداد مشخصی درخواست در بازه زمانی مشخص بفرسته.
//    /// </summary>
//    public class RateLimitMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<RateLimitMiddleware> _logger;

//        // دیکشنری که (IP + Path) → لیست زمان درخواست‌ها رو نگه می‌داره
//        private static readonly ConcurrentDictionary<string, ConcurrentQueue<long>> _requestLog = new();

//        // تنظیمات محدودسازی
//        private const int MaxRequests = 10; // حداکثر درخواست مجاز
//        private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(30); // بازه زمانی مجاز

//        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
//            var path = context.Request.Path.ToString().ToLowerInvariant();

//            var key = $"{ip}:{path}"; // کلید یکتا برای IP + مسیر

//            if (IsRateLimitExceeded(key))
//            {
//                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//                context.Response.Headers["Retry-After"] = TimeWindow.TotalSeconds.ToString();
//                await context.Response.WriteAsync($"Too many requests for this endpoint. Please wait {TimeWindow.TotalSeconds} seconds.");
//                _logger.LogWarning("Rate limit exceeded for {Ip} on {Path}", ip, path);
//                return;
//            }

//            await _next(context);
//        }

//        private bool IsRateLimitExceeded(string key)
//        {
//            var now = DateTime.UtcNow.Ticks;
//            var queue = _requestLog.GetOrAdd(key, _ => new ConcurrentQueue<long>());

//            // اضافه کردن زمان فعلی درخواست
//            queue.Enqueue(now);

//            // حذف درخواست‌هایی که از بازه زمانی ۳۰ ثانیه گذشته‌اند
//            var windowTicks = TimeWindow.Ticks;
//            while (queue.TryPeek(out var oldest) && (now - oldest) > windowTicks)
//            {
//                queue.TryDequeue(out _);
//            }

//            // بررسی اینکه آیا تعداد درخواست‌ها بیش از حد مجاز شده یا نه
//            return queue.Count > MaxRequests;
//        }
//    }

//    /// <summary>
//    /// اکستنشن برای افزودن Middleware به pipeline
//    /// </summary>
//    //public static class RateLimitMiddlewareExtensions
//    //{
//    //    public static IApplicationBuilder UseCustomRateLimiter(this IApplicationBuilder app)
//    //    {
//    //        return app.UseMiddleware<RateLimitMiddleware>();
//    //    }
//    //}
//}

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace JwtApi.Api.Middleware
{
    /// <summary>
    /// Middleware برای محدودسازی تعداد درخواست‌ها (Rate Limiting)
    /// هر کاربر لاگین‌شده (userId از JWT) فقط می‌تونه تعداد مشخصی درخواست در بازه زمانی مشخص بفرسته.
    /// اگر کاربر لاگین نکرده باشد، محدودیت بر اساس IP اعمال می‌شود.
    /// </summary>
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;

        // دیکشنری که (Key) → لیست زمان درخواست‌ها رو نگه می‌داره
        // Key = userId یا IP + مسیر
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<long>> _requestLog = new();

        // تنظیمات محدودسازی
        private const int MaxRequests = 10; // حداکثر درخواست مجاز
        private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(30); // بازه زمانی مجاز

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLowerInvariant();

            // تلاش برای گرفتن userId از JWT
            var userId = context.User?.FindFirst("id")?.Value;

            string key = userId != null ? $"user:{userId}:{path}" : $"ip:{context.Connection.RemoteIpAddress}:{path}";

            if (IsRateLimitExceeded(key))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = TimeWindow.TotalSeconds.ToString();
                await context.Response.WriteAsync($"Too many requests for this endpoint. Please wait {TimeWindow.TotalSeconds} seconds.");
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                return;
            }

            await _next(context);
        }

        private bool IsRateLimitExceeded(string key)
        {
            var now = DateTime.UtcNow.Ticks;
            var queue = _requestLog.GetOrAdd(key, _ => new ConcurrentQueue<long>());

            // اضافه کردن زمان فعلی درخواست
            queue.Enqueue(now);

            // حذف درخواست‌هایی که از بازه زمانی گذشته‌اند
            var windowTicks = TimeWindow.Ticks;
            while (queue.TryPeek(out var oldest) && (now - oldest) > windowTicks)
            {
                queue.TryDequeue(out _);
            }

            // بررسی تعداد درخواست‌ها
            return queue.Count > MaxRequests;
        }
    }

    /// <summary>
    /// اکستنشن برای افزودن Middleware به pipeline
    /// </summary>
    //public static class RateLimitMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseCustomRateLimiter(this IApplicationBuilder app)
    //    {
    //        return app.UseMiddleware<RateLimitMiddleware>();
    //    }
    //}
}



