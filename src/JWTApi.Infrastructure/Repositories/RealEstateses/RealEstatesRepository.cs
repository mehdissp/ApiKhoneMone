using Dapper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
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


        //public async Task<RealEstateDetails> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        //{
        //    var realEstate = await _context.RealEstates.Include(s=>s.Region).FirstOrDefaultAsync(s => s.Id == id);
        //    var query = await _context.RealEstates.FirstOrDefaultAsync(s => s.Id == id);
        //    var images = await _context.Images.Where(s => s.RealEstateId == id).Select(s => s.FullAddress).ToArrayAsync();
        //    var warnings = await _context.Warnings.Where(s => s.CategoryId == realEstate.CategoryId).Select(s => s.DescriptionRows).ToArrayAsync();
        //    var facilities = (from r in _context.RealEstates_Facilities
        //                      join f in _context.Facilities on r.FacilitiesId equals f.Id
        //                      where r.RealEstatesId == id
        //                      select f.Name)
        //      .ToArray();
        //    return new RealEstateDetails
        //    {
        //        IsHasStoreRoom = realEstate.IsHasStoreRoom,
        //        IsHasLoan = realEstate.IsHaLoan,
        //        Images = images,
        //        Warnings = warnings,
        //        Facilities = facilities,
        //        Floor = realEstate.Floor,
        //        CountFloor = realEstate.CountFloor,
        //        Address = realEstate.Address,
        //        AdditionalInformation = realEstate.AdditionalInformation,
        //        CreatedAt = realEstate.CreatedAt,
        //        ConstructionYear = realEstate.ConstructionYear,
        //        IsHasElevator = realEstate.IsHasElevator,
        //        IsHasParking = realEstate.IsHasParking,
        //        lat = realEstate.Latitude,
        //        lng = realEstate.Longitude,
        //        Price = (long)realEstate.Price,
        //        Title = realEstate.Title,
        //        IsHasPool = realEstate.IsHasPool,
        //        views = 0,
        //        RegionName=realEstate.Region.Name,

        //    };
        //}


        public async Task<RealEstateDetails> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        {
           // var realEstate = await _context.RealEstates.Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var realEstate = await _context.RealEstates.Include(s=>s.Category).Include(s => s.Region).FirstOrDefaultAsync(s => s.Id == id);
            var images = await _context.Images.Where(s => s.RealEstateId == id).AsNoTracking().Select(s => s.FullAddress).ToArrayAsync();
            var warnings = await _context.Warnings.Where(s => s.CategoryId == realEstate.CategoryId).AsNoTracking().Select(s => s.DescriptionRows).ToArrayAsync();
            var facilities = (from r in _context.RealEstates_Facilities
                              join f in _context.Facilities on r.FacilitiesId equals f.Id
                              where r.RealEstatesId == id
                              select f.Name)
              .ToArray();
            var agent = new
            {
                name = "مهدی",
                ConnectSocialMedia = "09190870450",
                Phone = "02144816283",
                Address = "تهران-جنت آباد ",
                Image = "blob:https://web.bale.ai/170ebfd3-b81b-4300-8054-7bd93e429f06"
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
                saved =1,
                RegionName = realEstate.Region.Name,
                DescriptionRows=realEstate.DescriptionRows,
                Agents =new Agent
                {
                    Name=agent.name,
                    Address=agent.Address,
                    Image=agent.Image,
                    ConnectSocialMedia=agent.ConnectSocialMedia,
                    Phone=agent.Phone,
                }

            };
        }

        //public async Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken)
        //{
        //    var query = from realEstate in _context.RealEstates
        //                where realEstate.UserId.ToString() == userId
        //                join image in _context.Images on realEstate.Id equals image.RealEstateId into imagesGroup
        //                select new RealEstatePanel
        //                {
        //                    Id = realEstate.Id,
        //                    Title = realEstate.Title,
        //                    Region = realEstate.Region != null ? realEstate.Region.Name : null,
        //                    Address = realEstate.Address,
        //                    Price = realEstate.Price,
        //                    Area = realEstate.SquareMeter,
        //                    CountRooms = realEstate.RoomCount,
        //                    CountFloor = realEstate.CountFloor,
        //                    Floor = realEstate.Floor,
        //                    IsHasParking = realEstate.IsHasParking,
        //                    IsHasElavator = realEstate.IsHasElevator,
        //                    IsHasLoan = realEstate.IsHaLoan,
        //                   // Status = realEstate.Status,
        //                    Views = "10",
        //                    CreatedAt = realEstate.CreatedAt,
        //                    Images = imagesGroup.Select(i => i.FullAddress).ToArray()
        //                };

        //    return await query.ToListAsync(cancellationToken);
        //}


        private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30); // کش 30 ثانیه

        public async Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = $"RealEstatePanel_{userId}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

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
            }) ?? new List<RealEstatePanel>();
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


        //public async Task<List<RegionDtos>> GetRegionsWithChildFlagAsync(int? id,CancellationToken cancellationToken)
        //{
        //    var allRegions = await _context.Regions
        //        .AsNoTracking()
        //        .Where(r => r.ParentId == id)
        //        .Select(r => new Region
        //        {
        //            Id = r.Id,
        //            Name = r.Name,
        //            Latitude = r.Latitude,
        //            Longitude = r.Longitude,
        //            ParentId = r.ParentId
        //        })
        //        .ToListAsync();

        //    // اگر دیتایی وجود نداشت، لیست خالی برگردان
        //    if (!allRegions.Any())
        //        return new List<RegionDtos>();

        //    // اعمال منطق تبدیل
        //    return RegionHelper.GetRegionsWithChildFlag(allRegions);
        //}
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
        public async Task InsertRealEstate(RealEstates realEstates, List<int> facilityIds,List<ImagesInfo> images)
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
            }
            catch (Exception ex)
            {

                throw;
            }
            // بدون تراکنش صریح، از تراکنش خودکار EF Core استفاده می‌شود
           
        }
    }
}

