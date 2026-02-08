using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Net;
using Microsoft.AspNetCore.Http.Features;
namespace JWTApi.Infrastructure.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
 
        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var permissionAttribute = endpoint.Metadata.GetMetadata<RequirePermissionAttribute>();
            if (permissionAttribute == null)
            {
                await _next(context);
                return;
            }

            var userPermissions = context.User?.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList() ?? new List<string>();

            if (!userPermissions.Contains(permissionAttribute.PermissionCode))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied: missing permission.");
                return;
            }

            await _next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute
    {
        public string PermissionCode { get; }

        public RequirePermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode;
        }
    }
}
