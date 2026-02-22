using Dapper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Interfaces.RealEstates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.RealEstates
{
    public class RealEstatesRepository : IRealEstatesRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RealEstatesRepository> _logger;
        private const string LastItemsCacheKey = "last_realestates_tab_{0}_page_{1}_size_{2}";

        public RealEstatesRepository(
            IDbConnection connection,
            IMemoryCache cache,
            ILogger<RealEstatesRepository> logger)
        {
            _connection = connection;
            _cache = cache;
            _logger = logger;
        }
        public async Task<List<RealEstateDto>> GetRandomLastItemRealEstates(int tabId, CancellationToken cancellation)
        {
            var query = @"
SELECT TOP 5 
    s.id,
    s.ConstructionYear,
    s.CountFloor,
    s.Title,
    s.AdditionalInformation,
    s.IsHasElevator,
    s.IsHasParking,
    s.IsHasPool,
    s.IsHasStoreRoom,
    r.Name,
    q.Name + ' / ' + ra.Name as ParentName,
    i.address,
    c.CategoryType
FROM dbo.RealEstates s
LEFT JOIN dbo.Regions r ON r.id = s.RegionId
LEFT JOIN dbo.Regions ra ON ra.id = r.ParentId
LEFT JOIN dbo.Regions q ON q.id = ra.ParentId
LEFT JOIN dbo.images i ON i.RealEstateId = s.id
INNER JOIN dbo.Categories c ON c.id = s.CategoryId
WHERE c.CategoryType = @TabId
ORDER BY s.Id DESC";

            var command = new CommandDefinition(
                query,
                parameters: new { TabId = tabId },
                cancellationToken: cancellation
            );

            var result = await _connection.QueryAsync<RealEstateDto>(command);
            return result.ToList();
        }


        public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
            int tabId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // اعتبارسنجی سریع
            if (tabId <= 0 || pageNumber < 1 || pageSize < 1 || pageSize > 50)
            {
                return new PagedResult<RealEstateWithCategoryDto>
                {
                    Items = new List<RealEstateWithCategoryDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }

            // تلاش برای دریافت از کش
            var cacheKey = string.Format(LastItemsCacheKey, tabId, pageNumber, pageSize);
            if (_cache.TryGetValue(cacheKey, out PagedResult<RealEstateWithCategoryDto> cachedResult))
            {
                return cachedResult;
            }

            try
            {
                // بهینه‌سازی کوئری برای پرفورمنس بالا
                var query = @"
DECLARE @TotalCount INT; 

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

-- دریافت تعداد کل با بهینه‌سازی
SELECT @TotalCount = COUNT(*)
FROM dbo.RealEstates s
INNER JOIN dbo.Categories c ON c.id = s.CategoryId
WHERE c.CategoryType = @TabId;

-- دریافت داده‌های صفحه جاری با ایندکس بهینه
SELECT 
    s.id,
    s.ConstructionYear,
    s.CountFloor,
    s.Title,
    s.AdditionalInformation,
    s.IsHasElevator,
    s.IsHasParking,
    s.IsHasPool,
    s.IsHasStoreRoom,
    r.Name as RegionName,
    q.Name + ' / ' + ra.Name as ParentName,
    i.address,
    ISNULL(img.ImageCount, 0) as ImageCount  -- تعداد کل عکس‌ها
,s.Price,
s.CreatedAt
FROM dbo.RealEstates s WITH (NOLOCK)
INNER JOIN dbo.Categories c WITH (NOLOCK) ON c.id = s.CategoryId
LEFT JOIN dbo.Regions r WITH (NOLOCK) ON r.id = s.RegionId
LEFT JOIN dbo.Regions ra WITH (NOLOCK) ON ra.id = r.ParentId
LEFT JOIN dbo.Regions q WITH (NOLOCK) ON q.id = ra.ParentId
OUTER APPLY (
    SELECT TOP 1 address as Address
    FROM dbo.images i WITH (NOLOCK)
    WHERE i.RealEstateId = s.id 
	and i.isbanner=1
    ORDER BY i.id
) i
LEFT JOIN (
    SELECT RealEstateId, COUNT(*) as ImageCount
    FROM dbo.images WITH (NOLOCK)
    GROUP BY RealEstateId
) img ON img.RealEstateId = s.id
WHERE c.Id = @tabId
ORDER BY s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

SELECT @TotalCount;";

                var parameters = new
                {
                    TabId = tabId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using (var multi = await _connection.QueryMultipleAsync(
                    query,
                    parameters,
                    commandTimeout: 5, // تایم‌اوت ۵ ثانیه
                    commandType: CommandType.Text))
                {
                    var items = (await multi.ReadAsync<RealEstateWithCategoryDto>()).ToList();
                    var totalCount = await multi.ReadFirstAsync<int>();

                    var result = new PagedResult<RealEstateWithCategoryDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    };

                    // کش کردن نتیجه برای ۲ دقیقه
                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت املاک برای tabId: {TabId}", tabId);

                // برگشت نتیجه خالی در صورت خطا
                return new PagedResult<RealEstateWithCategoryDto>
                {
                    Items = new List<RealEstateWithCategoryDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }
        }
    }
}

