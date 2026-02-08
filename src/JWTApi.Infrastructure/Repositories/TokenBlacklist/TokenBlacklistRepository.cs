using JWTApi.Domain.Interfaces.TokenBlacklist;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.TokenBlacklist
{
   public class TokenBlacklistRepository:ITokenBlacklistRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public TokenBlacklistRepository(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<bool> IsTokenBlacklistedAsync(string tokenId)
        {
            return _cache.TryGetValue($"blacklisted_token_{tokenId}", out _);
        }

        public async Task BlacklistTokenAsync(string tokenId, DateTime expiry)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiry
            };
            _cache.Set($"blacklisted_token_{tokenId}", true, cacheOptions);
        }

        public async Task BlacklistAllUserTokensAsync(string userId)
        {
            // این متد می‌تواند تمام توکن‌های کاربر را بلاک کند
            // برای پیاده‌سازی کامل‌تر نیاز به ذخیره‌سازی توکن‌ها دارید
            _cache.Set($"blacklisted_user_{userId}", true, TimeSpan.FromDays(30));
        }
    }
}
