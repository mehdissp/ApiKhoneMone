using JWTApi.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, JwtService tokenService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var (isValid, userId) = await tokenService.ValidateAndExtractUserIdAsync(token);

            if (!isValid)
            {
                context.Response.StatusCode = 401;

                // اگر کاربر غیرفعال شده باشد
                if (!string.IsNullOrEmpty(userId))
                {
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "User is deactivated",
                        code = "USER_DEACTIVATED"
                    });
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Invalid or expired token",
                        code = "INVALID_TOKEN"
                    });
                }
                return;
            }
        }

        await _next(context);
    }
}