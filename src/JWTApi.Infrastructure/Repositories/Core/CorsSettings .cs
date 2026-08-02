using JWTApi.Domain.Interfaces.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Core
{
    public class CorsSettings : ICorsSettings
    {
        private readonly IConfiguration _configuration;

        public CorsSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string[] GetAllowedOrigins()
        {
            return _configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
                   ?? Array.Empty<string>();
        }
    }
}
