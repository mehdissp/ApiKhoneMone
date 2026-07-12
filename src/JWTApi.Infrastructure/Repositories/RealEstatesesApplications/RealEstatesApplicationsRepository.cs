using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstatesesApplications;
using JWTApi.Domain.Interfaces.RealEstatesApplications;
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
        public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
            string userId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var userIdGuid = Guid.Parse(userId);

            // کوئری اصلی
            var query = from r in _context.RealEstatesApplicants
                        join q in _context.Regions on r.RegionId equals q.Id
                        join c in _context.Categories on r.CategoryId equals c.Id
                        join up in _context.RealEstatesApplicants_UserPaids
                            on new { r.Id, UserId = userIdGuid }
                            equals new { Id = up.RealEstatesApplicantsId, up.UserId } into paidJoin
                        from paid in paidJoin.DefaultIfEmpty()
                        select new RealEstatesesApplicationsDto
                        {
                            Title = r.Title,
                            Code = r.Code,
                            CreatedAt = r.CreatedAt,
                            RegionName = q.Name,
                            CategoryName = c.Name,
                            Regions = (from w in _context.RealEstatesApplicants_Regions
                                       join s in _context.Regions on w.RegionId equals s.Id
                                       where w.RealEstatesApplicantsId == r.Id
                                       select s.Name).ToArray(),
                            IsPaid = paid != null,
                            MobileNumber = paid != null ?
                                (from u in _context.Users where u.Id == userIdGuid select u.MobileNumber).FirstOrDefault()
                                : null
                        };

            // دریافت تعداد کل رکوردها
            var totalCount = await query.CountAsync(cancellationToken);

            // اعمال پیج بندی
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // محاسبه تعداد کل صفحات
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // بازگرداندن نتیجه پیج بندی شده
            return new PagedResult<RealEstatesesApplicationsDto>
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
