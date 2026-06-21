using JWTApi.Domain.Dtos.Stories;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Stories;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Stories
{
    // Repositories/StoryRepository.cs




    public class StoryRepository : IStoryRepository
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public StoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // دریافت استوری بر اساس ID
        public async Task<Story?> GetByIdAsync(int id)
        {
            return await _context.Stories
                .Include(s => s.RealEstates)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Story?> GetByIdByUserIdAsync(int id,string userId)
        {
            return await _context.Stories
                .Include(s => s.RealEstates)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId.ToString()== userId);
        }

        // دریافت استوری‌های فعال (منقضی نشده) برای یک ملک خاص
        public async Task<IEnumerable<Story>> GetActiveStoriesByPropertyAsync(int propertyId)
        {
            return await _context.Stories
                .Where(s => s.RealEstatesId == propertyId && s.ExpiresAt > DateTime.UtcNow)
                .Include(s => s.RealEstates)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        // دریافت همه استوری‌های منقضی شده
        public async Task<IEnumerable<Story>> GetAllExpiredStoriesAsync()
        {
            return await _context.Stories
                .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();
        }

        // بررسی وجود استوری
        public async Task<bool> StoryExistsAsync(int id)
        {
            return await _context.Stories.AnyAsync(s => s.Id == id);
        }

        // اضافه کردن استوری جدید
        public async Task<Story>  AddAsync(Story story)
        {
            // ExpiresAt توسط دیتابیس محاسبه میشه
            story.CreatedAt = DateTime.UtcNow;
            await _context.Stories.AddAsync(story);
            await SaveChangesAsync();
            // بارگذاری مجدد با relation
            await _context.Entry(story)
                .Reference(s => s.RealEstates)
                .LoadAsync();

            return story;
        }

        // اضافه کردن چند استوری با هم
        public async Task AddRangeAsync(IEnumerable<Story> stories)
        {
            foreach (var story in stories)
            {
                story.CreatedAt = DateTime.UtcNow;
            }
            await _context.Stories.AddRangeAsync(stories);
        }

        // حذف استوری
        public async Task<bool> DeleteAsync(int id,string userId,string roleName)
        {
            if (roleName=="Admin")
            {
                var story = await GetByIdAsync(id);
                if (story == null)
                    return false;

                _context.Stories.Remove(story);
                await SaveChangesAsync();
                return true;
            }
            else
            {
                var story = await GetByIdByUserIdAsync(id,userId);
                if (story == null)
                    return false;

                _context.Stories.Remove(story);
                await SaveChangesAsync();
                return true;
            }
        
        }

        // حذف همه استوری‌های منقضی شده (برمیگردونه تعداد حذف شده)
        public async Task<int> DeleteExpiredStoriesAsync()
        {
            var expiredStories = await GetAllExpiredStoriesAsync();
            _context.Stories.RemoveRange(expiredStories);
            var count = expiredStories.Count();
            await SaveChangesAsync();
            return count;
        }

        // حذف همه استوری‌های یک ملک خاص
        public async Task<int> DeleteByPropertyAsync(int propertyId)
        {
            var stories = await _context.Stories
                .Where(s => s.RealEstatesId == propertyId)
                .ToListAsync();

            _context.Stories.RemoveRange(stories);
            await SaveChangesAsync();
            return stories.Count;
        }

        // ذخیره تغییرات
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public async Task<List<StoryProfileDto>> StoryProfileDtos(string userId,CancellationToken  cancellationToken)
        {
            var stories = await _context.Stories
                .Include(s => s.RealEstates)
                .Where(s => s.UserId.ToString() == userId && s.ExpiresAt >=DateTime.Now)
                .ToListAsync(cancellationToken);

            var result = new List<StoryProfileDto>();

            foreach (var s in stories)
            {
                var dto = new StoryProfileDto
                {
                    Id = s.Id,
                    Title = s.Desc,
                    IsRealEstate = s.RealEstates != null,
                    UrlImage = s.RealEstates !=null ? await _context.Images.Where(w=>w.RealEstateId==s.RealEstatesId).Select(s=>s.FullAddress).FirstAsync() : s.ImagePath ,
                    RealEstateId = s.RealEstates != null ? s.RealEstates.Id : null,
                    TitleReal = s.RealEstates != null ? s.RealEstates.Title : null,
                    LinkReal = s.RealEstates != null ? $"property/{s.RealEstates.Id}/{s.RealEstates.Title}" : null
                };

                if (s.RealEstates != null)
                {
                    var image = await _context.Images
                        .FirstOrDefaultAsync(i => i.RealEstateId == s.RealEstates.Id, cancellationToken);
                    dto.ImgReal = image?.Address;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<List<StoryForSite>> GetStoriesDtos(CancellationToken cancellationToken)
        {
            // کوئری اول: گرفتن استوری‌های فعال
            var stories = await _context.Stories.Include(s=>s.RealEstates)
                .Where(s => s.Status == StoryStatusEnum.Accept && s.ExpiresAt >= DateTime.Now)
                .ToListAsync(cancellationToken);

            if (!stories.Any())
                return new List<StoryForSite>();

            // گرفتن UserId های منحصر به فرد از استوری‌ها
            var userIds = stories.Select(s => s.UserId).Distinct().ToList();

            // کوئری دوم: گرفتن یوزرهای مربوطه
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            // ترکیب دوتا دیتا در مموری
            var result = stories
                .GroupBy(s => s.UserId)
                .Select(g => new StoryForSite
                {
                    Id = g.Key.ToString(),
                    Name = users.ContainsKey(g.Key) ? users[g.Key].Name : null,
                    Avatar = users.ContainsKey(g.Key) ? users[g.Key].Avatar : null,
                    StoryUser = g.Select(s => new StoryUser
                    {
                        Id = s.Id,
                        Name = users.ContainsKey(s.UserId) ? users[s.UserId].Name : null,
                        Url = s.RealEstates != null ?  _context.Images.Where(w => w.RealEstateId == s.RealEstates.Id && w.IsBanner==true).Select(s => s.FullAddress).First() : s.ImagePath,
                     
                        Caption = s.Desc,
                        Link = s.RealEstates != null ? $"property/{s.RealEstates.Id}/{EncodeUrlPart(s.RealEstates.Title)}"
        : null,
                        LinkText = s.RealEstates != null ? "مشاهده آگهی"
        : null
                    }).ToList()
                })
                .ToList();

            return result;
        }


        public async Task<List<StoryForSite>> GetStoriesDtosForUser(string userId,CancellationToken cancellationToken)
        {
            // کوئری اول: گرفتن استوری‌های فعال
            var stories = await _context.Stories.Include(s => s.RealEstates)
                .Where(s => s.Status == StoryStatusEnum.Accept && s.ExpiresAt >= DateTime.Now && s.UserId.ToString()== userId)
                .ToListAsync(cancellationToken);

            if (!stories.Any())
                return new List<StoryForSite>();

            // گرفتن UserId های منحصر به فرد از استوری‌ها
            var userIds = stories.Select(s => s.UserId).Distinct().ToList();

            // کوئری دوم: گرفتن یوزرهای مربوطه
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            // ترکیب دوتا دیتا در مموری
            var result = stories
                .GroupBy(s => s.UserId)
                .Select(g => new StoryForSite
                {
                    Id = g.Key.ToString(),
                    Name = users.ContainsKey(g.Key) ? users[g.Key].Name : null,
                    Avatar = users.ContainsKey(g.Key) ? users[g.Key].Avatar : null,
                    StoryUser = g.Select(s => new StoryUser
                    {
                        Id = s.Id,
                        Name = users.ContainsKey(s.UserId) ? users[s.UserId].Name : null,
                        Url = s.RealEstates != null ? _context.Images.Where(w => w.RealEstateId == s.RealEstates.Id && w.IsBanner == true).Select(s => s.FullAddress).First() : s.ImagePath,

                        Caption = s.Desc,
                        Link = s.RealEstates != null ? $"property/{s.RealEstates.Id}/{EncodeUrlPart(s.RealEstates.Title)}"
        : null,
                        LinkText = s.RealEstates != null ? "مشاهده آگهی"
        : null
                    }).ToList()
                })
                .ToList();

            return result;
        }
        private string EncodeUrlPart(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            // فقط فاصله‌ها رو به %20 تبدیل کن، بقیه کاراکترها رو Uri.EscapeDataString انجام میده
            return Uri.EscapeDataString(text);
        }
    }
}
