namespace JwtApi.Api.Middleware
{
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomRateLimiter(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
