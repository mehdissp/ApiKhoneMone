using JWTApi.Domain.Dtos;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



public class MenuPermissionMiddleware
{
    private readonly RequestDelegate _next;

    public MenuPermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext _context)
    {
        // مسیرهای عمومی (public) رو بدون چک کردن اجازه عبور بده
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var publicPaths = new[] { "/api/auth/login", "/api/auth/register", "/swagger" , "/api/auth/captcha", "/api/auth/verify-captcha", "/api/UserProfile/upload-photo", "/api/payment/request", "/api/payment/test", "/api/aqayepardakht/request", "/api/userprofile/uploadphoto" };
        if (publicPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }
        // اگر کاربر لاگین نکرده
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Access denied: You are not authenticated.");
            return;
        }

        var paths = context.Request.Path.Value?.ToLower() ?? "";

        // بررسی اینکه منویی با این مسیر در دیتابیس داریم یا نه
        var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Url != null && paths.StartsWith(m.Url.ToLower()));

        if (menu == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Access denied:url is not valid.");
            return;
        }

        var userId = context.User.FindFirst("id")!.Value;

        // نقش‌های کاربر
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId.ToString() == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // بررسی دسترسی به منو
        var hasAccess = await _context.RoleMenus
            .AnyAsync(rm => userRoles.Contains(rm.RoleId) && rm.MenuId == menu.Id);

        if (!hasAccess)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
           // throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            await context.Response.WriteAsync("Access denied: You do not have permission for this endpoint.");
            return;
        }
        // حالا بررسی دکمه/عملیات
        string actionCode = context.Request.Headers["X-Action-Code"].FirstOrDefault() ?? "";

        //if (!string.IsNullOrEmpty(actionCode))
        //{
        //    var allowedPermissions = await _context.RoleMenus
        //        .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.MenuId == menu.Id)
        //        .Select(rmp => rmp.Permission.Code)
        //        .ToListAsync();

        //    if (!allowedPermissions.Contains(actionCode))
        //    {
        //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //        await context.Response.WriteAsync($"Access denied to action '{actionCode}' on menu '{menu.Name}'.");
        //        return;
        //    }
        //}
        await _next(context);
    }
}
