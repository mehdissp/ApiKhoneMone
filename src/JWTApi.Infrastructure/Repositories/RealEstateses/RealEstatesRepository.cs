using Dapper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Helper;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.RealEstateses
{
    public class RealEstatesRepository : IRealEstatesRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RealEstatesRepository> _logger;
        private readonly AppDbContext _context;
        private const string LastItemsCacheKey = "last_realestates_tab_{0}_page_{1}_size_{2}";

        public RealEstatesRepository(
            IDbConnection connection,
            IMemoryCache cache,
            AppDbContext context,
            ILogger<RealEstatesRepository> logger)
        {
            _connection = connection;
            _cache = cache;
            _logger = logger;
            _context= context;
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


        //        public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
        //            int tabId,
        //            int pageNumber = 1,
        //            int pageSize = 10,
        //            CancellationToken cancellationToken = default)
        //        {
        //            // اعتبارسنجی سریع
        //            if (tabId <= 0 || pageNumber < 1 || pageSize < 1 || pageSize > 50)
        //            {
        //                return new PagedResult<RealEstateWithCategoryDto>
        //                {
        //                    Items = new List<RealEstateWithCategoryDto>(),
        //                    TotalCount = 0,
        //                    PageNumber = pageNumber,
        //                    PageSize = pageSize,
        //                    TotalPages = 0
        //                };
        //            }

        //            // تلاش برای دریافت از کش
        //            var cacheKey = string.Format(LastItemsCacheKey, tabId, pageNumber, pageSize);
        //            if (_cache.TryGetValue(cacheKey, out PagedResult<RealEstateWithCategoryDto> cachedResult))
        //            {
        //                return cachedResult;
        //            }

        //            try
        //            {
        //                // بهینه‌سازی کوئری برای پرفورمنس بالا
        //                var query = @"
        //DECLARE @TotalCount INT; 

        //DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        //-- دریافت تعداد کل با بهینه‌سازی
        //SELECT @TotalCount = COUNT(*)
        //FROM dbo.RealEstates s
        //INNER JOIN dbo.Categories c ON c.id = s.CategoryId
        //WHERE c.CategoryType = @TabId;

        //-- دریافت داده‌های صفحه جاری با ایندکس بهینه
        //SELECT 
        //    s.id,
        //    s.ConstructionYear,
        //    s.CountFloor,
        //    s.Title,
        //    s.AdditionalInformation,
        //    s.IsHasElevator,
        //    s.IsHasParking,
        //    s.IsHasPool,
        //    s.IsHasStoreRoom,
        //    r.Name as RegionName,
        //    q.Name + ' / ' + ra.Name as ParentName,
        //    i.address,
        //    ISNULL(img.ImageCount, 0) as ImageCount  -- تعداد کل عکس‌ها
        //,s.Price,
        //s.CreatedAt
        //FROM dbo.RealEstates s WITH (NOLOCK)
        //INNER JOIN dbo.Categories c WITH (NOLOCK) ON c.id = s.CategoryId
        //LEFT JOIN dbo.Regions r WITH (NOLOCK) ON r.id = s.RegionId
        //LEFT JOIN dbo.Regions ra WITH (NOLOCK) ON ra.id = r.ParentId
        //LEFT JOIN dbo.Regions q WITH (NOLOCK) ON q.id = ra.ParentId
        //OUTER APPLY (
        //    SELECT TOP 1 address as Address
        //    FROM dbo.images i WITH (NOLOCK)
        //    WHERE i.RealEstateId = s.id 
        //	and i.isbanner=1
        //    ORDER BY i.id
        //) i
        //LEFT JOIN (
        //    SELECT RealEstateId, COUNT(*) as ImageCount
        //    FROM dbo.images WITH (NOLOCK)
        //    GROUP BY RealEstateId
        //) img ON img.RealEstateId = s.id
        //WHERE c.Id = @tabId
        //ORDER BY s.Id DESC
        //OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        //SELECT @TotalCount;";

        //                var parameters = new
        //                {
        //                    TabId = tabId,
        //                    PageNumber = pageNumber,
        //                    PageSize = pageSize
        //                };

        //                using (var multi = await _connection.QueryMultipleAsync(
        //                    query,
        //                    parameters,
        //                    commandTimeout: 5, // تایم‌اوت ۵ ثانیه
        //                    commandType: CommandType.Text))
        //                {
        //                    var items = (await multi.ReadAsync<RealEstateWithCategoryDto>()).ToList();
        //                    var totalCount = await multi.ReadFirstAsync<int>();

        //                    var result = new PagedResult<RealEstateWithCategoryDto>
        //                    {
        //                        Items = items,
        //                        TotalCount = totalCount,
        //                        PageNumber = pageNumber,
        //                        PageSize = pageSize,
        //                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        //                    };

        //                    // کش کردن نتیجه برای ۲ دقیقه
        //                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));

        //                    return result;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "خطا در دریافت املاک برای tabId: {TabId}", tabId);

        //                // برگشت نتیجه خالی در صورت خطا
        //                return new PagedResult<RealEstateWithCategoryDto>
        //                {
        //                    Items = new List<RealEstateWithCategoryDto>(),
        //                    TotalCount = 0,
        //                    PageNumber = pageNumber,
        //                    PageSize = pageSize,
        //                    TotalPages = 0
        //                };
        //            }
        //        }

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
                isnull(q.Name,'') + ' / ' + ra.Name as ParentName,
            i.address,
            ISNULL(img.ImageCount, 0) as ImageCount,
            s.Price,
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
                    commandTimeout: 5,
                    commandType: CommandType.Text))
                {
                    var items = (await multi.ReadAsync<RealEstateWithCategoryDto>()).ToList();
                    var totalCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<RealEstateWithCategoryDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    };
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

        //    public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
        //int tabId,
        //int pageNumber = 1,
        //int pageSize = 10,
        //CancellationToken cancellationToken = default)
        //    {
        //        // اعتبارسنجی
        //        if (tabId <= 0 || pageNumber < 1 || pageSize < 1 || pageSize > 50)
        //            return new PagedResult<RealEstateWithCategoryDto>
        //            {
        //                Items = new List<RealEstateWithCategoryDto>(),
        //                TotalCount = 0,
        //                PageNumber = pageNumber,
        //                PageSize = pageSize,
        //                TotalPages = 0
        //            };

        //        try
        //        {
        //            // ✅ کوئری شمارش مجزا و بهینه
        //            var countQuery = @"
        //        SELECT COUNT(*)
        //        FROM dbo.RealEstates s
        //        INNER JOIN dbo.Categories c ON c.Id = s.CategoryId
        //        WHERE c.CategoryType = @TabId
        //          ";

        //            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new { TabId = tabId });

        //            if (totalCount == 0)
        //                return new PagedResult<RealEstateWithCategoryDto>
        //                {
        //                    Items = new List<RealEstateWithCategoryDto>(),
        //                    TotalCount = 0,
        //                    PageNumber = pageNumber,
        //                    PageSize = pageSize,
        //                    TotalPages = 0
        //                };

        //            // ✅ کوئری اصلی برای دریافت داده‌ها
        //            var dataQuery = @"
        //    WITH ImageSummary AS (
        //        SELECT 
        //            RealEstateId,
        //            COUNT(*) AS ImageCount,
        //            MAX(CASE WHEN isbanner = 1 THEN Id END) AS BannerImageId
        //        FROM dbo.images
        //        GROUP BY RealEstateId
        //    ),
        //    BannerImages AS (
        //        SELECT 
        //            RealEstateId,
        //            Address
        //        FROM dbo.images
        //        WHERE isbanner = 1
        //    )
        //    SELECT 
        //        s.Id,
        //        s.ConstructionYear,
        //        s.CountFloor,
        //        s.Title,
        //        s.AdditionalInformation,
        //        s.IsHasElevator,
        //        s.IsHasParking,
        //        s.IsHasPool,
        //        s.IsHasStoreRoom,
        //        s.Price,
        //        s.CreatedAt,
        //        r.Name AS RegionName,
        //        ISNULL(q.Name, '') + ' / ' + ISNULL(ra.Name, '') AS ParentName,
        //        ISNULL(bi.Address, '') AS [Address],
        //        ISNULL(img.ImageCount, 0) AS ImageCount
        //    FROM dbo.RealEstates s
        //    INNER JOIN dbo.Categories c ON c.Id = s.CategoryId
        //    LEFT JOIN dbo.Regions r ON r.Id = s.RegionId
        //    LEFT JOIN dbo.Regions ra ON ra.Id = r.ParentId
        //    LEFT JOIN dbo.Regions q ON q.Id = ra.ParentId
        //    LEFT JOIN ImageSummary img ON img.RealEstateId = s.Id
        //    LEFT JOIN BannerImages bi ON bi.RealEstateId = s.Id
        //    WHERE c.CategoryType = @TabId

        //    ORDER BY s.Id DESC
        //    OFFSET (@PageNumber - 1) * @PageSize ROWS
        //    FETCH NEXT @PageSize ROWS ONLY;";

        //            var items = (await _connection.QueryAsync<RealEstateWithCategoryDto>(
        //                dataQuery,
        //                new { TabId = tabId, PageNumber = pageNumber, PageSize = pageSize },
        //                commandTimeout: 10)).ToList();

        //            return new PagedResult<RealEstateWithCategoryDto>
        //            {
        //                Items = items,
        //                TotalCount = totalCount,
        //                PageNumber = pageNumber,
        //                PageSize = pageSize,
        //                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "خطا در دریافت املاک برای tabId: {TabId}", tabId);
        //            return new PagedResult<RealEstateWithCategoryDto>
        //            {
        //                Items = new List<RealEstateWithCategoryDto>(),
        //                TotalCount = 0,
        //                PageNumber = pageNumber,
        //                PageSize = pageSize,
        //                TotalPages = 0
        //            };
        //        }
        //    }



        public async Task<PagedResult<RealEstateMap>> GetRealStateMap(
int regionId,
int pageNumber = 1,
int pageSize = 10,
CancellationToken cancellationToken = default)
        {
            // اعتبارسنجی سریع
            if (regionId <= 0 || pageNumber < 1 || pageSize < 1 || pageSize > 50)
            {
                return new PagedResult<RealEstateMap>
                {
                    Items = new List<RealEstateMap>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
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
LEFT JOIN dbo.Regions r WITH (NOLOCK) ON r.id = s.RegionId
LEFT JOIN dbo.Regions ra WITH (NOLOCK) ON ra.id = r.ParentId
LEFT JOIN dbo.Regions q WITH (NOLOCK) ON q.id = ra.ParentId
WHERE ra.ParentId=@regionId
AND s.Latitude is not null

;

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
	s.Latitude as lat,
	s.Longitude as lng,
	case when c.CategoryType=1 then N'فروش'
	else N'رهن' end as TypeCate
	,
    r.Name as RegionName,
        isnull(q.Name,'') + ' / ' + ra.Name as ParentName,
    i.address,
    ISNULL(img.ImageCount, 0) as ImageCount,
    s.Price,
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
WHERE ra.ParentId=@regionId
AND s.Latitude is not null
ORDER BY s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

SELECT @TotalCount;";

                var parameters = new
                {
                    regionId = regionId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using (var multi = await _connection.QueryMultipleAsync(
                    query,
                    parameters,
                    commandTimeout: 5,
                    commandType: CommandType.Text))
                {
                    var items = (await multi.ReadAsync<RealEstateMap>()).ToList();
                    var totalCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<RealEstateMap>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت املاک برای tabId: {TabId}", regionId);

                // برگشت نتیجه خالی در صورت خطا
                return new PagedResult<RealEstateMap>
                {
                    Items = new List<RealEstateMap>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }
        }



        public async Task<RealEstateDetails> GetRealEstateDetails(int id,string? userId,CancellationToken cancellationToken)
        {


           // var realEstate = await _context.RealEstates.Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var realEstate = await _context.RealEstates.Include(s=>s.Category).Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var images = await _context.Images.Where(s => s.RealEstateId == id).AsNoTracking().Select(s => s.FullAddress).ToArrayAsync();
            var warnings = await _context.Warnings.Where(s => s.CategoryId == realEstate.CategoryId).AsNoTracking().Select(s => s.DescriptionRows).ToArrayAsync();
            var facilities = (from r in _context.RealEstates_Facilities
                              join f in _context.Facilities on r.FacilitiesId equals f.Id
                              where r.RealEstatesId == id
                              select f.Name).ToArray();

            var agents = await _context.Users.FirstOrDefaultAsync(s => s.Id == realEstate.UserId);

        var agent = new
            {
                name = agents.FullName,
                ConnectSocialMedia = agents.MobileNumber,
                Phone = agents.MobileNumber,
            Address = "تهران-جنت آباد ",
                Image = agents.Avatar,
                IsHasStory=await _context.Stories.AnyAsync(s=>s.UserId ==agents.Id && s.ExpiresAt >=DateTime.Now && s.Status== StoryStatusEnum.Accept)
        };
            return new RealEstateDetails
            {
                Id=realEstate.Id,
                CategoryType=(int)realEstate.Category.CategoryType,
                IsHasStoreRoom = realEstate.IsHasStoreRoom,
                IsHasLoan = realEstate.IsHaLoan,
                Images = images,
                Warnings = warnings,
                Facilities = facilities,
                Floor = realEstate.Floor,
                CountFloor = realEstate.CountFloor,
                Address = realEstate.Address,
                AdditionalInformation = realEstate.AdditionalInformation,
                CreatedAt = realEstate.CreatedAt,
                ConstructionYear = realEstate.ConstructionYear,
                IsHasElevator = realEstate.IsHasElevator,
                IsHasParking = realEstate.IsHasParking,
                lat = realEstate.Latitude,
                lng = realEstate.Longitude,
                Price = realEstate.Price,
                Deposit= realEstate.Deposit,
                Rent=realEstate.Rent,
                PriceMeter=realEstate.Price/realEstate.SquareMeter,
                ShowExactLocation=realEstate.IsShowLocation,
                Title = realEstate.Title,
                IsHasPool = realEstate.IsHasPool,
                views = 10,
                Rooms=realEstate.RoomCount,
                saved = await _context.BookMarks.CountAsync(s=>s.RealEstatesId== id),
                RegionName = realEstate.Region.Name,
                DescriptionRows=realEstate.DescriptionRows,
                InBookMark = !string.IsNullOrEmpty(userId) &&
             await _context.BookMarks.AnyAsync(s => s.UserId.ToString() == userId && s.RealEstatesId==id),
                Agents =new Agent
                {
                    Name= agents.FullName,
                    Address= agent.Address,
                    Image= agents.Avatar,
                    ConnectSocialMedia=agent.ConnectSocialMedia,
                    Phone=agents.MobileNumber,
                    HasStory=agent.IsHasStory,
                    UserId=agents.Id.ToString()
                }

            };
        }


        private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30); // کش 30 ثانیه

        public async Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken)
        {
          

       
             

                // همان کوئری بهینه بالا
                var realEstates = await _context.RealEstates
                    .AsNoTracking()
                    .Include(x => x.Region)
                    .Where(x => x.UserId.ToString() == userId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        RegionName = x.Region != null ? x.Region.Name : null,
                        x.Address,
                        x.Price,
                        x.SquareMeter,
                        x.RoomCount,
                        x.CountFloor,
                        x.Floor,
                        x.IsHasParking,
                        x.IsHasElevator,
                        x.IsHaLoan,
                        x.CreatedAt,
                        x.Status
                    })
                    .ToListAsync(cancellationToken);

                if (!realEstates.Any())
                    return new List<RealEstatePanel>();

                var realEstateIds = realEstates.Select(x => x.Id).ToList();

                var imagesDictionary = await _context.Images
                    .AsNoTracking()
                    .Where(x => realEstateIds.Contains(x.RealEstateId))
                    .GroupBy(x => x.RealEstateId)
                    .Select(g => new { RealEstateId = g.Key, Images = g.Select(i => i.FullAddress).ToArray() })
                    .ToDictionaryAsync(x => x.RealEstateId, x => x.Images, cancellationToken);

                return realEstates.Select(x => new RealEstatePanel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Region = x.RegionName,
                    Address = x.Address,
                    Price = x.Price,
                    Area = x.SquareMeter,
                    CountRooms = x.RoomCount,
                    CountFloor = x.CountFloor,
                    Floor = x.Floor,
                    IsHasParking = x.IsHasParking,
                    IsHasElavator = x.IsHasElevator,
                    IsHasLoan = x.IsHaLoan,
                    Views = "10",
                    CreatedAt = x.CreatedAt,
                    Status=x.Status.ToPersianString(),
                    Images = imagesDictionary.GetValueOrDefault(x.Id) ?? Array.Empty<string>()
                }).ToList();
           
        }

        public async Task<List<FacilitiesDtos>> GetFacilitiesDtos(int catId, CancellationToken cancellationToken)
        {
            return await _context.Facilities
      .Where(s => s.CategoryId == catId)
      .Select(s => new FacilitiesDtos
      {
          Id = s.Id,
          Name = s.Name,
          
          // سایر فیلدها...
      })
      .ToListAsync(cancellationToken);
        }


        public async Task<List<RegionDtos>> GetRegionsWithChildFlagAsync(int? id, CancellationToken cancellationToken)
        {
            var query = from s in _context.Regions
                        where s.ParentId == id
                        select new RegionDtos
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            HasRegion=s.HasRegion,
                            CountChild = _context.Regions.Any(a => a.ParentId == s.Id) ? 1 : 0
                        };

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<List<RegionDtos>> GetRegions( CancellationToken cancellationToken)
        {
            var query = from s in _context.Regions.Where(s=>s.ParentId==null)
                
                        select new RegionDtos
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            HasRegion = s.HasRegion,
                            CountChild = _context.Regions.Any(a => a.ParentId == s.Id) ? 1 : 0
                        };

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }


        public async Task<int> InsertRealEstate(RealEstates realEstates, List<int> facilityIds,List<ImagesInfo> images)
        {
            try
            {
                await _context.RealEstates.AddAsync(realEstates);
                await _context.SaveChangesAsync();

                // استفاده از AddRange برای یک SaveChanges
                var facilities = facilityIds.Select(fId => new RealEstates_Facilities
                {
                    RealEstatesId = realEstates.Id,
                    FacilitiesId = fId
                }).ToList();
                List<Image> imageList = images.Select((s, index) => new Image
                {
                    Name = s.FileName,
                    Address = s.Url,
                    FullAddress = s.Url,
                    IsBanner = index == 0 , // اولین آیتم true، بقیه false
                    RealEstateId=realEstates.Id,

                }).ToList();

                await _context.Set<RealEstates_Facilities>().AddRangeAsync(facilities);
                await _context.Set<Image>().AddRangeAsync(imageList);
                await _context.SaveChangesAsync();
                return realEstates.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
            // بدون تراکنش صریح، از تراکنش خودکار EF Core استفاده می‌شود
           
        }
        public async Task<bool> CheckAccessToRealEstate(int id, string userId, string roleName, CancellationToken cancellationToken)
        {
            if (roleName == "Admin")
            {
                return true;
            }
            else
            {
                return await _context.RealEstates
                    .AnyAsync(r => r.Id == id && r.UserId.ToString() == userId, cancellationToken);
            }
        }


        public async Task<RealEstateDetailsEdit> GetRealEstateDetailsForEdit(int id, CancellationToken cancellationToken)
        {


            // var realEstate = await _context.RealEstates.Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var realEstate = await _context.RealEstates.Include(s => s.Category).Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var images = await _context.Images.Where(s => s.RealEstateId == id).AsNoTracking().Select(s => s.FullAddress).ToArrayAsync();
       
            var facilities = (from r in _context.RealEstates_Facilities
                              join f in _context.Facilities on r.FacilitiesId equals f.Id
                              where r.RealEstatesId == id
                              select f.Name)
              .ToArray();
  
            return new RealEstateDetailsEdit
            {
                Id = realEstate.Id,
                CategoryType = (int)realEstate.Category.CategoryType,
                IsHasStoreRoom = realEstate.IsHasStoreRoom,
                IsHasLoan = realEstate.IsHaLoan,
                Images = images,
          
                Facilities = facilities,
                Floor = realEstate.Floor,
                CountFloor = realEstate.CountFloor,
                Address = realEstate.Address,
                AdditionalInformation = realEstate.AdditionalInformation,
                CreatedAt = realEstate.CreatedAt,
                ConstructionYear = realEstate.ConstructionYear,
                IsHasElevator = realEstate.IsHasElevator,
                IsHasParking = realEstate.IsHasParking,
                lat = realEstate.Latitude,
                lng = realEstate.Longitude,
                Price = realEstate.Price,
                Deposit = realEstate.Deposit,
                Rent = realEstate.Rent,
                PriceMeter = realEstate.Price / realEstate.SquareMeter,
                ShowExactLocation = realEstate.IsShowLocation,
                Title = realEstate.Title,
                IsHasPool = realEstate.IsHasPool,
                Rooms = realEstate.RoomCount,            
                RegionName = realEstate.Region.Name,
                DescriptionRows = realEstate.DescriptionRows,
                RegionId=realEstate.RegionId
            };
        }

        public async Task<RealEstates> GetRealEstates(int id ,CancellationToken cancellationToken)
        {
            return await _context.RealEstates.FindAsync(id, cancellationToken);
        }

        public async Task<AdPriceRanges> getAdPriceRange(Guid roleId, int categoryId)
        {
            return await _context.AdPriceRanges
                .FirstOrDefaultAsync(s => s.RoleId == roleId
                && s.IsActive == true
            && s.CategoryId == categoryId 
            && s.StartDate <= DateTime.Now 
            &&s.EndDate>= DateTime.Now
            );
        }

        //public async Task<PaymentStatusDtos> GetPaymentStatus(int realEstateId,Guid roleId,Guid userId)
        //{
        //    var realEstate = await _context.RealEstates.FindAsync(realEstateId);
        //    var wallet=await _context.Wallets.FirstOrDefaultAsync(s=>s.UserId==userId);
        //    var getAdPrice = await _context.AdPriceRanges
        //        .FirstOrDefaultAsync(s => s.RoleId == roleId
        //        && s.IsActive == true
        //        && s.CategoryId == realEstate.CategoryId
        //        && s.StartDate <= DateTime.Now
        //        && s.EndDate >= DateTime.Now
        //        );
        //    if (getAdPrice.AdPostingCost <= wallet.Balance)
        //    {
        //        return new PaymentStatusDtos
        //        {
        //            IsWalletPay = true,
        //            AdPrice = getAdPrice.AdPostingCost,
        //            WalletBalance = wallet.Balance,
        //            Debtor = 0

        //        };
        //    }
        //    return new PaymentStatusDtos
        //    {
        //        IsWalletPay = false,
        //        AdPrice = getAdPrice.AdPostingCost,
        //        WalletBalance = wallet.Balance,
        //        Debtor = getAdPrice.AdPostingCost - wallet.Balance

        //    };



        //}

        public async Task<PaymentStatusDtos> GetPaymentStatus(int realEstateId, Guid roleId, Guid userId)
        {
            // 1. دریافت اطلاعات ملک
            var realEstate = await _context.RealEstates
                .FirstOrDefaultAsync(s=>s.Id==realEstateId 
            && s.IsDeleted==false && s.Status== RealEstateStatusEnum.WaitingForPayment);
            if (realEstate == null)
            {
             
                    return new PaymentStatusDtos
                    {
                        IsWalletPay = false,
                        AdPrice = 0,
                        WalletBalance = 0,
                        Debtor = 0,
                        ErrorMessage = "ملک یافت نشد"
                    };
                
            }

            // 2. دریافت کیف پول کاربر
            var wallet = await _context.Wallets.FirstOrDefaultAsync(s => s.UserId == userId);
            if (wallet == null)
            {
                return new PaymentStatusDtos
                {
                    IsWalletPay = false,
                    AdPrice = 0,
                    WalletBalance = 0,
                    Debtor = 0,
                    ErrorMessage = "کیف پول کاربر یافت نشد"
                };
           
            }

            // 3. دریافت قیمت درج آگهی بر اساس نقش و دسته‌بندی ملک
            var getAdPrice = await _context.AdPriceRanges
                .FirstOrDefaultAsync(s => s.RoleId == roleId
                    && s.IsActive == true
                    && s.CategoryId == realEstate.CategoryId
                    && s.StartDate <= DateTime.Now
                    && s.EndDate >= DateTime.Now
                );

            if (getAdPrice == null)
            {
                return new PaymentStatusDtos
                {
                    IsWalletPay = false,
                    AdPrice = 0,
                    WalletBalance = 0,
                    Debtor = 0,
                    ErrorMessage = "تعرفه درج آگهی برای این دسته‌بندی و نقش فعال یافت نشد"
                };
                
            }

            // 4. محاسبه وضعیت پرداخت
            decimal adPrice = getAdPrice.AdPostingCost;
            decimal walletBalance = wallet.Balance;
            bool isWalletPay = adPrice <= walletBalance;
            decimal debtor = isWalletPay ? 0 : adPrice - walletBalance;

            return new PaymentStatusDtos
            {
                IsWalletPay = isWalletPay,
                AdPrice = adPrice,
                WalletBalance = walletBalance,
                Debtor = debtor
            };
        }

        public async Task InsertBookMark(BookMark bookMark ,CancellationToken cancellationToken)
        {
            await _context.BookMarks.AddAsync(bookMark,cancellationToken);
        }

        public async Task InsertViolations(Violation violation,CancellationToken cancellationToken)
        {
            await _context.Violations.AddAsync(violation, cancellationToken);

        }

        public async Task<bool> DeleteBookMark(BookMark bookMark, CancellationToken cancellationToken)
        {
            var bookmarksToDelete = await _context.BookMarks
                .Where(s => s.UserId == bookMark.UserId && s.RealEstatesId == bookMark.RealEstatesId)
                .ToListAsync(cancellationToken);

            if (!bookmarksToDelete.Any())
                return false;

            _context.BookMarks.RemoveRange(bookmarksToDelete);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


        public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithUser(
string userId,
int pageNumber = 1,
int pageSize = 10,
CancellationToken cancellationToken = default)
        {
            // اعتبارسنجی سریع
            if (userId is null || pageNumber < 1 || pageSize < 1 || pageSize > 50)
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
where s.userId=@userId


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
        isnull(q.Name,'') + ' / ' + ra.Name as ParentName,
    i.address,
    ISNULL(img.ImageCount, 0) as ImageCount,
    s.Price,
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
WHERE s.userId = @userId
ORDER BY s.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

SELECT @TotalCount;";

                var parameters = new
                {
                    userId = userId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using (var multi = await _connection.QueryMultipleAsync(
                    query,
                    parameters,
                    commandTimeout: 5,
                    commandType: CommandType.Text))
                {
                    var items = (await multi.ReadAsync<RealEstateWithCategoryDto>()).ToList();
                    var totalCount = await multi.ReadFirstAsync<int>();

                    return new PagedResult<RealEstateWithCategoryDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت املاک برای tabId: {TabId}", userId);

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




        public async Task<PagedResult<RealEstatePanel>> GetRealEstateBookMark(
    string userId,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            // اعتبارسنجی ورودی‌ها
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // محدودیت حداکثر

            // کوئری پایه
            var query = _context.BookMarks.Include(s=>s.RealEstates)
                .AsNoTracking()
                .Include(x => x.RealEstates.Region)
                .Where(x => x.UserId.ToString() == userId);

            // دریافت تعداد کل رکوردها برای محاسبه صفحات
            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<RealEstatePanel>
                {
                    Items = Array.Empty<RealEstatePanel>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }

            // دریافت داده‌های صفحه مورد نظر
            var realEstates = await query
                .OrderByDescending(x => x.CreatedAt) // مرتب‌سازی برای صفحه‌بندی پایدار
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.RealEstates.Id,
                    x.RealEstates.Title,
                    RegionName = x.RealEstates.Region != null ? x.RealEstates.Region.Name : null,
                   x.RealEstates.Address,
                   x.RealEstates.Price,
                   x.RealEstates.SquareMeter,
                   x.RealEstates.RoomCount,
                   x.RealEstates.CountFloor,
                   x.RealEstates.Floor,
                   x.RealEstates.IsHasParking,
                   x.RealEstates.IsHasElevator,
                   x.RealEstates.IsHaLoan,
                   x.RealEstates.CreatedAt,
                    x.RealEstates.Status
                })
                .ToListAsync(cancellationToken);

            // دریافت تصاویر برای آیتم‌های این صفحه
            var realEstateIds = realEstates.Select(x => x.Id).ToList();
            var imagesDictionary = await _context.Images
                .AsNoTracking()
                .Where(x => realEstateIds.Contains(x.RealEstateId))
                .GroupBy(x => x.RealEstateId)
                .Select(g => new
                {
                    RealEstateId = g.Key,
                    Images = g.Select(i => i.FullAddress).ToArray()
                })
                .ToDictionaryAsync(
                    x => x.RealEstateId,
                    x => x.Images,
                    cancellationToken);

            // ساخت آیتم‌های صفحه جاری
            var items = realEstates.Select(x => new RealEstatePanel
            {
                Id = x.Id,
                Title = x.Title,
                Region = x.RegionName,
                Address = x.Address,
                Price = x.Price,
                Area = x.SquareMeter,
                CountRooms = x.RoomCount,
                CountFloor = x.CountFloor,
                Floor = x.Floor,
                IsHasParking = x.IsHasParking,
                IsHasElavator = x.IsHasElevator,
                IsHasLoan = x.IsHaLoan,
                Views = "10",
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToPersianString(),
                Images = imagesDictionary.GetValueOrDefault(x.Id) ?? Array.Empty<string>()
            }).ToList();

            // محاسبه تعداد کل صفحات
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResult<RealEstatePanel>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }


    }
}

