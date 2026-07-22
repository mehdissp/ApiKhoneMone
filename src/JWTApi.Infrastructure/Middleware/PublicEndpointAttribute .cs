using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Middleware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PublicEndpointAttribute : Attribute
    {
        public bool AllowPublic { get; set; } = true;
    }
}
