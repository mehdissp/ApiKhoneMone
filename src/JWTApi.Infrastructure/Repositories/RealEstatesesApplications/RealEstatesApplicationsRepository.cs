using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstatesesApplications;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.RealEstatesApplications;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Repositories.RealEstateses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.RealEstatesesApplications
{
    public class RealEstatesApplicationsRepository: IRealEstatesApplicationsRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RealEstatesRepository> _logger;
        private readonly AppDbContext _context;
        private const string LastItemsCacheKey = "last_realestates_tab_{0}_page_{1}_size_{2}";
        public RealEstatesApplicationsRepository(
             IDbConnection connection,
             IMemoryCache cache,
             AppDbContext context,
             ILogger<RealEstatesRepository> logger)
        {
            _connection = connection;
            _cache = cache;
            _logger = logger;
            _context = context;
        }
        //public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
        //    string userId,
        //    int pageNumber = 1,
        //    int pageSize = 10,
        //    CancellationToken cancellationToken = default)
        //{
        //    var userIdGuid = Guid.Parse(userId);

        //    // کوئری اصلی
        //    var query = from r in _context.RealEstatesApplicants
        //                join q in _context.Regions on r.RegionId equals q.Id
        //                join c in _context.Categories on r.CategoryId equals c.Id
        //                join up in _context.RealEstatesApplicants_UserPaids
        //                    on new { r.Id, UserId = userIdGuid }
        //                    equals new { Id = up.RealEstatesApplicantsId, up.UserId } into paidJoin
        //                from paid in paidJoin.DefaultIfEmpty()
        //                select new RealEstatesesApplicationsDto
        //                {
        //                    Id=r.Id,
        //                    Title = r.Title,
        //                    Code = r.Code,
        //                    CreatedAt = r.CreatedAt,
        //                    RegionName = q.Name,
        //                    Desc=r.Desc,
        //                    Budget=r.Budget,
        //                    CategoryName = c.Name,
        //                    Regions = (from w in _context.RealEstatesApplicants_Regions
        //                               join s in _context.Regions on w.RegionId equals s.Id
        //                               where w.RealEstatesApplicantsId == r.Id
        //                               select s.Name).ToArray(),
        //                    IsPaid = paid != null,
        //                    MobileNumber = paid != null ?
        //                        (from u in _context.Users where u.Id == userIdGuid select u.MobileNumber).FirstOrDefault()
        //                        : null
        //                };

        //    // دریافت تعداد کل رکوردها
        //    var totalCount = await query.CountAsync(cancellationToken);

        //    // اعمال پیج بندی
        //    var items = await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .AsNoTracking()
        //        .ToListAsync(cancellationToken);

        //    // محاسبه تعداد کل صفحات
        //    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //    // بازگرداندن نتیجه پیج بندی شده
        //    return new PagedResult<RealEstatesesApplicationsDto>
        //    {
        //        Items = items,
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalPages = totalPages
        //    };
        //}

        //    public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
        //string userId,
        //string searchTerm = null,
        //int pageNumber = 1,
        //int pageSize = 10,
        //CancellationToken cancellationToken = default)
        //    {
        //        try
        //        {
        //            var userIdGuid = Guid.Parse(userId);

        //            // کوئری اصلی با تمام JOIN ها
        //            var query = from r in _context.RealEstatesApplicants
        //                        join q in _context.Regions on r.RegionId equals q.Id
        //                        join c in _context.Categories on r.CategoryId equals c.Id
        //                        join up in _context.RealEstatesApplicants_UserPaids
        //                            on new { r.Id, UserId = userIdGuid }
        //                            equals new { Id = up.RealEstatesApplicantsId, UserId = up.UserId } into paidJoin
        //                        from paid in paidJoin.DefaultIfEmpty()
        //                        select new
        //                        {
        //                            r.Id,
        //                            r.Title,
        //                            r.Code,
        //                            r.CreatedAt,
        //                            r.Desc,
        //                            r.Budget,
        //                            RegionName = q.Name,
        //                            CategoryName = c.Name,
        //                            IsPaid = paid != null,
        //                            MobileNumber = paid != null ?
        //                                _context.Users.Where(u => u.Id == userIdGuid).Select(u => u.MobileNumber).FirstOrDefault()
        //                                : null,
        //                            // برای جستجو در Regions یک SubQuery
        //                            RegionsNames = _context.RealEstatesApplicants_Regions
        //                                .Where(w => w.RealEstatesApplicantsId == r.Id)
        //                                .Join(_context.Regions,
        //                                      w => w.RegionId,
        //                                      s => s.Id,
        //                                      (w, s) => s.Name)
        //                                .ToList()
        //                        };

        //            // اعمال فیلتر سرچ (با پشتیبانی از جستجوی ترکیبی)
        //            if (!string.IsNullOrWhiteSpace(searchTerm))
        //            {
        //                searchTerm = searchTerm.Trim();
        //                query = query.Where(x =>
        //                    EF.Functions.Like(x.Title, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.ToString(), $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.Desc, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.RegionName, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.CategoryName, $"%{searchTerm}%") ||
        //                    (x.MobileNumber != null && EF.Functions.Like(x.MobileNumber, $"%{searchTerm}%")) ||
        //                    x.RegionsNames.Any(r => EF.Functions.Like(r, $"%{searchTerm}%"))
        //                );
        //            }

        //            // انتخاب نهایی با تبدیل به DTO
        //            var finalQuery = query.Select(x => new RealEstatesesApplicationsDto
        //            {
        //                Id = x.Id,
        //                Title = x.Title,
        //                Code = x.Code,
        //                CreatedAt = x.CreatedAt,
        //                RegionName = x.RegionName,
        //                Desc = x.Desc,
        //                Budget = x.Budget,
        //                CategoryName = x.CategoryName,
        //                Regions = x.RegionsNames.ToArray(),
        //                IsPaid = x.IsPaid,
        //                MobileNumber = x.MobileNumber
        //            });

        //            // دریافت تعداد کل
        //            var totalCount = await finalQuery.CountAsync(cancellationToken);

        //            // دریافت آیتم‌های صفحه
        //            var items = await finalQuery
        //                .Skip((pageNumber - 1) * pageSize)
        //                .Take(pageSize)
        //                .AsNoTracking()
        //                .ToListAsync(cancellationToken);

        //            return new PagedResult<RealEstatesesApplicationsDto>
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

        //            throw;
        //        }

        //    }


        //    public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
        //string userId,
        //string searchTerm = null,
        //int pageNumber = 1,
        //int pageSize = 10,
        //CancellationToken cancellationToken = default)
        //    {
        //        try
        //        {
        //            var userIdGuid = Guid.Parse(userId);

        //            // Step 1: دریافت اطلاعات اصلی
        //            var baseQuery = from r in _context.RealEstatesApplicants
        //                            join q in _context.Regions on r.RegionId equals q.Id
        //                            join c in _context.Categories on r.CategoryId equals c.Id
        //                            select new
        //                            {
        //                                r.Id,
        //                                r.Title,
        //                                r.Code,
        //                                r.CreatedAt,
        //                                r.Desc,
        //                                r.Budget,
        //                                RegionName = q.Name,
        //                                CategoryName = c.Name
        //                            };

        //            // Step 2: اعمال فیلترهای ساده
        //            if (!string.IsNullOrWhiteSpace(searchTerm))
        //            {
        //                searchTerm = searchTerm.Trim();
        //                baseQuery = baseQuery.Where(x =>
        //                    EF.Functions.Like(x.Title, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.Desc, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.RegionName, $"%{searchTerm}%") ||
        //                    EF.Functions.Like(x.CategoryName, $"%{searchTerm}%")
        //                );
        //            }

        //            // Step 3: اجرای کوئری اصلی
        //            var baseResults = await baseQuery.ToListAsync(cancellationToken);
        //            var ids = baseResults.Select(x => x.Id).ToList();

        //            // Step 4: دریافت اطلاعات اضافی در چند کوئری مجزا
        //            var paidInfo = await _context.RealEstatesApplicants_UserPaids
        //                .Where(up => ids.Contains(up.RealEstatesApplicantsId) && up.UserId == userIdGuid)
        //                .ToDictionaryAsync(up => up.RealEstatesApplicantsId, cancellationToken);

        //            var regionsInfo = await _context.RealEstatesApplicants_Regions
        //                .Where(w => ids.Contains(w.RealEstatesApplicantsId))
        //                .Join(_context.Regions,
        //                      w => w.RegionId,
        //                      s => s.Id,
        //                      (w, s) => new { w.RealEstatesApplicantsId, s.Name })
        //                .GroupBy(x => x.RealEstatesApplicantsId)
        //                .Select(g => new { Id = g.Key, Names = g.Select(x => x.Name).ToList() })
        //                .ToDictionaryAsync(x => x.Id, x => x.Names, cancellationToken);

        //            var mobileNumbers = await _context.Users
        //                .Where(u => u.Id == userIdGuid)
        //                .Select(u => u.MobileNumber)
        //                .FirstOrDefaultAsync(cancellationToken);

        //            // Step 5: ترکیب اطلاعات
        //            var finalResults = baseResults.Select(x => new RealEstatesesApplicationsDto
        //            {
        //                Id = x.Id,
        //                Title = x.Title,
        //                Code = x.Code,
        //                CreatedAt = x.CreatedAt,
        //                RegionName = x.RegionName,
        //                Desc = x.Desc,
        //                Budget = x.Budget,
        //                CategoryName = x.CategoryName,
        //                Regions = regionsInfo.TryGetValue(x.Id, out var regions) ? regions.ToArray() : Array.Empty<string>(),
        //                IsPaid = paidInfo.ContainsKey(x.Id),
        //                MobileNumber = paidInfo.ContainsKey(x.Id) ? mobileNumbers : null
        //            }).ToList();

        //            // Step 6: اعمال فیلتر جستجوی پیشرفته روی Regions
        //            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 1)
        //            {
        //                finalResults = finalResults
        //                    .Where(x => x.Regions.Any(r => r.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        //                    .ToList();
        //            }

        //            // Step 7: پیاده‌سازی Pagination
        //            var totalCount = finalResults.Count;
        //            var items = finalResults
        //                .Skip((pageNumber - 1) * pageSize)
        //                .Take(pageSize)
        //                .ToList();

        //            return new PagedResult<RealEstatesesApplicationsDto>
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
        //            // لاگ کردن خطا
        //            throw;
        //        }
        //    }

        public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
    string userId,
    string searchTerm = null,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var userIdGuid = Guid.Parse(userId);

                // Step 1: دریافت اطلاعات اصلی
                var baseQuery = from r in _context.RealEstatesApplicants
                                join q in _context.Regions on r.RegionId equals q.Id
                                join c in _context.Categories on r.CategoryId equals c.Id
                                join w in _context.Users on r.UserId equals w.Id
                                select new
                                {
                                    r.Id,
                                    r.Title,
                                    r.Code,
                                    r.CreatedAt,
                                    r.Desc,
                                    r.Budget,
                                    RegionName = q.Name,
                                    CategoryName = c.Name,
                                    w.MobileNumber
                                };

                // Step 2: اعمال فیلترهای ساده (قسمتی که در SQL اجرا می‌شود)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.Trim();
                    baseQuery = baseQuery.Where(x =>
                        EF.Functions.Like(x.Title, $"%{searchTerm}%") ||
                        EF.Functions.Like(x.Desc, $"%{searchTerm}%") ||
                        EF.Functions.Like(x.RegionName, $"%{searchTerm}%") ||
                        EF.Functions.Like(x.CategoryName, $"%{searchTerm}%")
                    );
                }

                // Step 3: اجرای کوئری اصلی
                var baseResults = await baseQuery.ToListAsync(cancellationToken);
                var ids = baseResults.Select(x => x.Id).ToList();

                // Step 4: دریافت اطلاعات اضافی در چند کوئری مجزا
                var paidInfo = await _context.RealEstatesApplicants_UserPaids
                    .Where(up => ids.Contains(up.RealEstatesApplicantsId) && up.UserId == userIdGuid)
                    .ToDictionaryAsync(up => up.RealEstatesApplicantsId, cancellationToken);

                var regionsInfo = await _context.RealEstatesApplicants_Regions
                    .Where(w => ids.Contains(w.RealEstatesApplicantsId))
                    .Join(_context.Regions,
                          w => w.RegionId,
                          s => s.Id,
                          (w, s) => new { w.RealEstatesApplicantsId, s.Name })
                    .GroupBy(x => x.RealEstatesApplicantsId)
                    .Select(g => new { Id = g.Key, Names = g.Select(x => x.Name).ToList() })
                    .ToDictionaryAsync(x => x.Id, x => x.Names, cancellationToken);

                var mobileNumbers = await _context.Users
                    .Where(u => u.Id == userIdGuid)
                    .Select(u => u.MobileNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                // Step 5: ترکیب اطلاعات
                var finalResults = baseResults.Select(x => new RealEstatesesApplicationsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    RegionName = x.RegionName,
                    Desc = x.Desc,
                    Budget = x.Budget,
                    CategoryName = x.CategoryName,
                    Regions = regionsInfo.TryGetValue(x.Id, out var regions) ? regions.ToArray() : Array.Empty<string>(),
                    IsPaid = paidInfo.ContainsKey(x.Id),
                    MobileNumber = paidInfo.ContainsKey(x.Id) ? x.MobileNumber : null
                }).ToList();

                // Step 6: اگر جستجو وجود دارد و در Regions هم باید جستجو شود
                // اینجا باید شرط را به صورت OR به نتایج اضافه کنیم، نه اینکه جایگزین کنیم
                if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 1)
                {
                    // نتایجی که در Step 2 پیدا شده‌اند، نگه داشته می‌شوند
                    // و نتایجی که در Regions مطابقت دارند، به آنها اضافه می‌شوند
                    var regionSearchResults = await _context.RealEstatesApplicants_Regions
                        .Where(w => _context.Regions.Any(r => r.Id == w.RegionId && EF.Functions.Like(r.Name, $"%{searchTerm}%")))
                        .Select(w => w.RealEstatesApplicantsId)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    // اضافه کردن آیتم‌هایی که در Regions پیدا شده‌اند و قبلاً در finalResults نیستند
                    var existingIds = finalResults.Select(x => x.Id).ToHashSet();
                    var newIds = regionSearchResults.Where(id => !existingIds.Contains(id)).ToList();

                    if (newIds.Any())
                    {
                        // دریافت اطلاعات کامل برای آیتم‌های جدید
                        var newItemsQuery = from r in _context.RealEstatesApplicants
                                            join q in _context.Regions on r.RegionId equals q.Id
                                            join c in _context.Categories on r.CategoryId equals c.Id
                                            join w in _context.Users on r.UserId equals w.Id
                                            where newIds.Contains(r.Id)
                                            select new
                                            {
                                                r.Id,
                                                r.Title,
                                                r.Code,
                                                r.CreatedAt,
                                                r.Desc,
                                                r.Budget,
                                                RegionName = q.Name,
                                                CategoryName = c.Name,
                                                w.MobileNumber,
                                            };

                        var newBaseResults = await newItemsQuery.ToListAsync(cancellationToken);
                        var newIdsList = newBaseResults.Select(x => x.Id).ToList();

                        // دریافت اطلاعات اضافی برای آیتم‌های جدید
                        var newPaidInfo = await _context.RealEstatesApplicants_UserPaids
                            .Where(up => newIdsList.Contains(up.RealEstatesApplicantsId) && up.UserId == userIdGuid)
                            .ToDictionaryAsync(up => up.RealEstatesApplicantsId, cancellationToken);

                        var newRegionsInfo = await _context.RealEstatesApplicants_Regions
                            .Where(w => newIdsList.Contains(w.RealEstatesApplicantsId))
                            .Join(_context.Regions,
                                  w => w.RegionId,
                                  s => s.Id,
                                  (w, s) => new { w.RealEstatesApplicantsId, s.Name })
                            .GroupBy(x => x.RealEstatesApplicantsId)
                            .Select(g => new { Id = g.Key, Names = g.Select(x => x.Name).ToList() })
                            .ToDictionaryAsync(x => x.Id, x => x.Names, cancellationToken);

                        // ساخت آیتم‌های جدید
                        var newItems = newBaseResults.Select(x => new RealEstatesesApplicationsDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Code = x.Code,
                            CreatedAt = x.CreatedAt,
                            RegionName = x.RegionName,
                            Desc = x.Desc,
                            Budget = x.Budget,
                            CategoryName = x.CategoryName,
                            Regions = newRegionsInfo.TryGetValue(x.Id, out var regionsNew) ? regionsNew.ToArray() : Array.Empty<string>(),
                            IsPaid = newPaidInfo.ContainsKey(x.Id),
                            MobileNumber = newPaidInfo.ContainsKey(x.Id) ? x.MobileNumber : null
                        }).ToList();

                        // اضافه کردن آیتم‌های جدید به لیست نهایی
                        finalResults.AddRange(newItems);
                    }
                }

                // Step 7: پیاده‌سازی Pagination
                var totalCount = finalResults.Count;
                var items = finalResults
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PagedResult<RealEstatesesApplicationsDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا
                throw;
            }
        }


        public async Task<RealEstatesesApplicationsDetailsDto> GetRealEstatesesApplicationsDetails(int id, string userId, CancellationToken cancellationToken)
        {
            var userIdGuid = Guid.Parse(userId);

            var query = from r in _context.RealEstatesApplicants
                        where r.Id == id  // ✅ فیلتر اصلی
                        join q in _context.Regions on r.RegionId equals q.Id
                        join c in _context.Categories on r.CategoryId equals c.Id
                        join w in _context.Users on r.UserId equals w.Id
                        join up in _context.RealEstatesApplicants_UserPaids
                    
                            on new { Id = r.Id, UserId = userIdGuid }
                            equals new { Id = up.RealEstatesApplicantsId, UserId = up.UserId } into paidJoin
                        from paid in paidJoin.DefaultIfEmpty()
                        select new RealEstatesesApplicationsDetailsDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Code = r.Code,
                            CreatedAt = r.CreatedAt,
                            RegionName = q.Name,
                            Desc = r.Desc,
                            Budget = r.Budget,
                            CategoryName = c.Name,
                            MaxConstructionYear = r.MaxConstructionYear,
                            MaxSquareMeter=r.MaxSquareMeter,
                            MinCountRoom=r.MinCountRoom,
                            MinConstructionYear=r.MinConstructionYear,
                            MinSquareMeter=r.MinSquareMeter,
                            Regions = _context.RealEstatesApplicants_Regions
                                .Where(w => w.RealEstatesApplicantsId == r.Id)
                                .Join(_context.Regions,
                                      w => w.RegionId,
                                      s => s.Id,
                                      (w, s) => s.Name)
                                .ToArray(),
                            IsPaid = paid != null,
                            MobileNumber = paid != null ?
                           w.MobileNumber
                                : null,
                            FullNameCustomer = paid != null ?
                                w.FullName
                                : null,
                        };

            return await query.FirstOrDefaultAsync(cancellationToken);
        }



        public async Task<PaymentStatusDtos> GetPaymentStatus(int realEstateIdAppId, Guid roleId, Guid userId,CancellationToken cancellationToken)
        {
            // 1. دریافت اطلاعات ملک
            var realEstate = await _context.RealEstatesApplicants
                .FirstOrDefaultAsync(s => s.Id == realEstateIdAppId
            && s.IsDeleted == false, cancellationToken);
            if (realEstate == null)
            {

                return new PaymentStatusDtos
                {
                    IsWalletPay = false,
                    AdPrice = 0,
                    WalletBalance = 0,
                    Debtor = 0,
                    ErrorMessage = "درخواستی  یافت نشد "
                };

            }

            // 2. دریافت کیف پول کاربر
            var wallet = await _context.Wallets.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
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
                    && s.AdPriceRangeType == AdPriceRangeType.ShowApplicantRequest, cancellationToken
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


        public async Task AccessToShowMobileNumber(int realEstateIdAppId, Guid roleId, Guid userId,CancellationToken cancellationToken)
        {
           RealEstatesApplicants_UserPaid realEstatesApplicants = new RealEstatesApplicants_UserPaid();
            realEstatesApplicants.UserId= userId;
            realEstatesApplicants.RealEstatesApplicantsId=realEstateIdAppId;
            await _context.AddAsync(realEstatesApplicants, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
