using JWTApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace JWTApi.Api.Middleware
{
public class LoginRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public LoginRateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (context.Request.Path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userIdClaim = context.User?.FindFirst("id")?.Value;
            Guid? userId = null;
            if (Guid.TryParse(userIdClaim, out var uid)) userId = uid;

            // بررسی lockout کاربر
            if (userId != null)
            {
                var lockout = await db.IpLocks.FindAsync(userId.Value);
                if (lockout != null && lockout.LockEnd > DateTime.Now)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync($"Account locked. Try again at {lockout.LockEnd} UTC.");
                    return;
                }
            }

            // شمارش تلاش‌های ناموفق در 1 دقیقه گذشته
            var fromTime = DateTime.Now - Window;
            var failedAttempts = await db.LoginAttempts
                .Where(x => x.IPAddress == ip && !x.Success && x.AttemptTime >= fromTime)
                .CountAsync();

            if (failedAttempts >= MaxFailedAttempts)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many failed login attempts. Try again later.");
                return;
            }
        }

        await _next(context);
    }
}

}
