using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.TokenBlacklist
{
   public interface ITokenBlacklistRepository
    {
        Task<bool> IsTokenBlacklistedAsync(string tokenId);
        Task BlacklistTokenAsync(string tokenId, DateTime expiry);
        Task BlacklistAllUserTokensAsync(string userId);
    }
}
