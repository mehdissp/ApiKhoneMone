using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTApi.Domain.Dtos.Stories;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Stories;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                .Where(s => s.UserId.ToString() == userId)
                .ToListAsync(cancellationToken);

            var result = new List<StoryProfileDto>();

            foreach (var s in stories)
            {
                var dto = new StoryProfileDto
                {
                    Id = s.Id,
                    Title = s.Desc,
                    IsRealEstate = s.RealEstates != null,
                    UrlImage = s.ImagePath,
                    RealEstateId = s.RealEstates != null ? s.RealEstates.Id : null,
                    TitleReal = s.RealEstates != null ? s.RealEstates.Title : null,
                    LinkReal = s.RealEstates != null ? "ss" : null
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

        //public async Task<List<StoriesDtos>> GetStoriesDtos(CancellationToken cancellationToken)
        //{
        //    return await
        //}
    }
}
