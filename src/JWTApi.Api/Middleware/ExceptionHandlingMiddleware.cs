//using JWTApi.Api.Response;
//using JWTApi.Infrastructure.Exceptions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading.Tasks;

//namespace JWTApi.Api.Middleware
//{
//    public class ExceptionHandlingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

//        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (RestBasedException ex)
//            {
//                _logger.LogWarning(ex, "RestBasedException occurred: {Message}", ex.Message);
//                await HandleRestBasedExceptionAsync(context, ex);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
//                await HandleGenericExceptionAsync(context, ex);
//            }
//        }

//        private static async Task HandleRestBasedExceptionAsync(HttpContext context, RestBasedException ex)
//        {
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = ex.HttpStatus;


//            var response = ResponseApi.Error(ex.Message, ex.HttpStatus);
//            await context.Response.WriteAsJsonAsync(response);


//        }

//        private static async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
//        {
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = 500;

//            var response = ResponseApi.Error("Internal server error.", 500);
//            await context.Response.WriteAsJsonAsync(response);
//        }
//    }

//    public static class ExceptionHandlingMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
//        {
//            return app.UseMiddleware<ExceptionHandlingMiddleware>();
//        }
//    }
//}
using JWTApi.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JWTApi.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RestBasedException ex)
            {
                _logger.LogWarning(ex, "RestBasedException occurred: {Message}", ex.Message);
                await HandleRestBasedExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private static async Task HandleRestBasedExceptionAsync(HttpContext context, RestBasedException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.HttpStatus;

            var response = new
            {
                status = ex.HttpStatus,
                data = new
                {
                    message = ex.Message
                }
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = new
            {
                status = 500,
                data = new
                {
                    message = "Internal server error."
                }
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}


