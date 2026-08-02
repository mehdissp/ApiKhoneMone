using JWTApi.Domain.Interfaces.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Middleware
{
    //public class AllowedOriginsAttribute : Attribute, IAsyncActionFilter
    //{
    //    private readonly string[] _allowedOrigins;

    //    public AllowedOriginsAttribute(params string[] allowedOrigins)
    //    {
    //        _allowedOrigins = allowedOrigins;
    //    }

    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        var request = context.HttpContext.Request;
    //        var origin = request.Headers["Origin"].ToString();
    //        var referer = request.Headers["Referer"].ToString();

    //        // بررسی Origin یا Referer
    //        var isAllowed = _allowedOrigins.Any(allowed =>
    //            origin?.StartsWith(allowed) == true ||
    //            referer?.StartsWith(allowed) == true
    //        );

    //        if (!isAllowed)
    //        {
    //            context.Result = new ObjectResult(new { Message = "دسترسی غیرمجاز" })
    //            {
    //                StatusCode = 403
    //            };
    //            return;
    //        }

    //        await next();
    //    }
    //}

    //public class AllowedOriginsAttribute : Attribute, IAsyncActionFilter
    //{
    //    private readonly string[] _allowedOrigins;

    //    public AllowedOriginsAttribute(ICorsSettings corsSettings)
    //    {
    //        _allowedOrigins = corsSettings.GetAllowedOrigins();
    //    }

    //    public AllowedOriginsAttribute(params string[] allowedOrigins)
    //    {
    //        _allowedOrigins = allowedOrigins;
    //    }

    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        var request = context.HttpContext.Request;
    //        var origin = request.Headers["Origin"].ToString();
    //        var referer = request.Headers["Referer"].ToString();

    //        var isAllowed = _allowedOrigins.Any(allowed =>
    //            origin?.StartsWith(allowed) == true ||
    //            referer?.StartsWith(allowed) == true
    //        );

    //        if (!isAllowed)
    //        {
    //            context.Result = new ObjectResult(new { Message = "دسترسی غیرمجاز" })
    //            {
    //                StatusCode = 403
    //            };
    //            return;
    //        }

    //        await next();
    //    }
    //}

    public class AllowedOriginsAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[]? _customAllowedOrigins;

        public AllowedOriginsAttribute() { }

        public AllowedOriginsAttribute(params string[] allowedOrigins)
        {
            _customAllowedOrigins = allowedOrigins;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var corsSettings = context.HttpContext.RequestServices.GetRequiredService<ICorsSettings>();
            var allowedOrigins = _customAllowedOrigins ?? corsSettings.GetAllowedOrigins();

            var request = context.HttpContext.Request;
            var origin = request.Headers["Origin"].ToString();
            var referer = request.Headers["Referer"].ToString();

            var isAllowed = allowedOrigins.Any(allowed =>
                origin?.StartsWith(allowed) == true ||
                referer?.StartsWith(allowed) == true
            );

            if (!isAllowed)
            {
                context.Result = new ObjectResult(new { Message = "دسترسی غیرمجاز" })
                {
                    StatusCode = 403
                };
                return;
            }

            await next();
        }
    }
}
