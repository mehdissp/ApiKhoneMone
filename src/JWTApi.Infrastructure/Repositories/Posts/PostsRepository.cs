using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Post;
using JWTApi.Domain.Entities.Blogs;
using JWTApi.Domain.Interfaces.Posts;
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

namespace JWTApi.Infrastructure.Repositories.Posts;
public class PostsRepository : IPostsRepository
{
    private readonly IDbConnection _connection;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RealEstatesRepository> _logger;
    private readonly AppDbContext _context;
    private const string LastItemsCacheKey = "last_realestates_tab_{0}_page_{1}_size_{2}";
    public PostsRepository(
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

    public async Task<List<CategoryPostsDTOS>> GetCategoryPostsDTOs(CancellationToken cancellationToken)
    {
        // کلید کش (یونیک باشه)
        string cacheKey = "CategoryPostsDTOs_All";

        // بررسی آیا داده توی کش وجود داره؟
        if (_cache.TryGetValue(cacheKey, out List<CategoryPostsDTOS> cachedData))
        {
            return cachedData; // اگر داشت، از کش برگردون
        }

        // اگر نداشت، از دیتابیس بگیر
        var data = await _context.CategoryPosts
            .Select(s => new CategoryPostsDTOS
            {
                Id = s.Id,
                Name = s.Name,
                Slug = s.Slug
            })
            .ToListAsync(cancellationToken);

        // ذخیره توی کش با مدت زمان ۵ ساعت
        _cache.Set(cacheKey, data, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddHours(5),
            Priority = CacheItemPriority.Normal
        });

        return data;
    }


    public async Task<List<TagsDtos>> GetTagsDtosDTOs(int catId, CancellationToken cancellationToken)
    {


        // اگر نداشت، از دیتابیس بگیر
        var data = await _context.Tags.Where(s => s.CategoryId == catId)
            .Select(s => new TagsDtos
            {
                Id = s.Id,
                Name = s.Name,

            })
            .ToListAsync(cancellationToken);



        return data;
    }
    public async Task InsertPost(Post post, int[] tagIds, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Posts.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. حالا که Id داریم، Tagها را اضافه کنید
            if (tagIds != null && tagIds.Any())
            {
                var existingTags = await _context.Tags
                    .Where(t => tagIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                var post_Tags = existingTags.Select(tag => new Post_Tags
                {
                    PostId = post.Id,  // الان Id وجود دارد
                    TagsId = tag.Id
                }).ToList();

                await _context.Post_Tags.AddRangeAsync(post_Tags, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);



            }

        }
        catch (Exception ex)
        {

            throw;
        }
        // 1. ابتدا پست را ذخیره کنید تا Id تولید شود

    }


    public async Task<List<PostCategoryDto>> GetTopViewedPostsAsync(
        CancellationToken cancellationToken )
    {
        var cacheKey = $"TopViewedPosts_{10}";

        // بررسی وجود در کش
        if (_cache.TryGetValue(cacheKey, out List<PostCategoryDto> cachedPosts))
        {
            return cachedPosts;
        }

        // دریافت از دیتابیس
        var posts = await _context.Posts
            .Include(s => s.Category)
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.ViewCount)
            .Take(10)
            .Select(s => new PostCategoryDto
            {
                Id = s.Id,
                Title = s.Title,
                Slug = s.Slug,
                Summary = s.Summary,
                CategoryPostName = s.Category.Name,
                CountView = s.ViewCount,
                CreatedAt = s.CreatedAt,
                ImageUrl = s.ImageUrl,
            })
            .ToListAsync(cancellationToken);

        // ذخیره در کش با انقضای 10 دقیقه
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(10))
            .SetPriority(CacheItemPriority.High);

        _cache.Set(cacheKey, posts, cacheOptions);

        return posts;
    }

    public async Task<List<PostCategoryDto>> GetTopNewPostsAsync(
      CancellationToken cancellationToken)
    {
        var cacheKey = $"TopNewPosts_{10}";

        // بررسی وجود در کش
        if (_cache.TryGetValue(cacheKey, out List<PostCategoryDto> cachedPosts))
        {
            return cachedPosts;
        }

        // دریافت از دیتابیس
        var posts = await _context.Posts
            .Include(s => s.Category)
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.Id)
            .Take(10)
            .Select(s => new PostCategoryDto
            {
                Id = s.Id,
                Title = s.Title,
                Slug = s.Slug,
                Summary = s.Summary,
                CategoryPostName = s.Category.Name,
                CountView = s.ViewCount,
                CreatedAt = s.CreatedAt,
                ImageUrl = s.ImageUrl,
            })
            .ToListAsync(cancellationToken);

        // ذخیره در کش با انقضای 10 دقیقه
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(10))
            .SetPriority(CacheItemPriority.High);

        _cache.Set(cacheKey, posts, cacheOptions);

        return posts;
    }

    public async Task<PagedResult<PostCategoryDto>> GetPostCategoryDto
        (
    int pageNumber = 1,
    int pageSize = 10,
    string searchTerm = null,
    int? categoryId = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            // شروع کوئری با Include های لازم
            var query = _context.Posts
                .Include(s => s.Category)
                .AsQueryable();

            // اعمال فیلتر بر اساس CategoryId
            if (categoryId.HasValue)
            {
                query = query.Where(s => s.CategoryId == categoryId.Value);
            }

            // اعمال سرچ (جستجو در Title و Summary و Slug)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(s =>
                    s.Title.Contains(searchTerm) ||
                    s.Summary.Contains(searchTerm) ||
                    s.Slug.Contains(searchTerm)
                );
            }

            // محاسبه تعداد کل آیتم‌ها قبل از صفحه‌بندی
            var totalCount = await query.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی
            var items = await query
                .OrderByDescending(s => s.CreatedAt) // مرتب‌سازی بر اساس جدیدترین
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new PostCategoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Slug = s.Slug,
                    Summary = s.Summary,
                    CategoryPostName = s.Category.Name,
                    CountView = s.ViewCount,
                    CreatedAt = s.CreatedAt,
                    ImageUrl = s.ImageUrl,
                    IsPublished=s.IsPublished,
                })
                .ToListAsync(cancellationToken);

            // ایجاد نتیجه صفحه‌بندی شده
            return new PagedResult<PostCategoryDto>
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
            // لاگ خطا
            throw new Exception($"خطا در دریافت پست‌ها: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<PostCategoryDto>> GetPostCategoryDtoAdmin
    (
int pageNumber = 1,
int pageSize = 10,
string searchTerm = null,
int? categoryId = null,
bool? isPublish=null,
CancellationToken cancellationToken = default)
    {
        try
        {
            // شروع کوئری با Include های لازم
            var query = _context.Posts
                .Include(s => s.Category)
                .AsQueryable();

            // اعمال فیلتر بر اساس CategoryId
            if (categoryId.HasValue)
            {
                query = query.Where(s => s.CategoryId == categoryId.Value);
            }
            if (isPublish !=null)
            {
                query = query.Where(s => s.IsPublished == isPublish);
            }
            if (isPublish == null)
            {
                query = query.Where(s => s.IsPublished==false || s.IsPublished==true);
            }

            //        query = query.Where(s => s.IsPublished == isPublish);


            // اعمال سرچ (جستجو در Title و Summary و Slug)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(s =>
                    s.Title.Contains(searchTerm) ||
                    s.Summary.Contains(searchTerm) ||
                    s.Slug.Contains(searchTerm)
                );
            }

            // محاسبه تعداد کل آیتم‌ها قبل از صفحه‌بندی
            var totalCount = await query.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی
            var items = await query
                .OrderByDescending(s => s.CreatedAt) // مرتب‌سازی بر اساس جدیدترین
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new PostCategoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Slug = s.Slug,
                    Summary = s.Summary,
                    CategoryPostName = s.Category.Name,
                    CountView = s.ViewCount,
                    CreatedAt = s.CreatedAt,
                    ImageUrl = s.ImageUrl,
                    IsPublished = s.IsPublished,
                })
                .ToListAsync(cancellationToken);

            // ایجاد نتیجه صفحه‌بندی شده
            return new PagedResult<PostCategoryDto>
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
            // لاگ خطا
            throw new Exception($"خطا در دریافت پست‌ها: {ex.Message}", ex);
        }
    }

    public async Task<PostDetailsDtos> GetDetailsDtosAsync(int id, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(s => s.Category)
       
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new PostDetailsDtos
            {
                Id = s.Id,
                Title = s.Title,
                Slug = s.Slug,
                Content = s.Content,
                Summary = s.Summary,
                CategoryPostName = s.Category.Name,
                CountView = s.ViewCount,
                CreatedAt = s.CreatedAt,
                ImageUrl = s.ImageUrl,
                Agents = null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return null;

        // Get tags separately
        var tags = await _context.Post_Tags
            .Where(pt => pt.PostId == id)
            .Join(_context.Tags,
                  pt => pt.TagsId,
                  t => t.Id,
                  (pt, t) => t.Name)
            .ToArrayAsync(cancellationToken);

        post.Tags = tags;
        return post;
    }

    public async Task UpdateViewCount(int id, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FindAsync(id, cancellationToken);
        if (post == null) return;

        // اگر ViewCount نال بود، مقدار 0 بده
        post.ViewCount = (post.ViewCount ?? 0) + 1;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatus(int id,bool type,CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FindAsync(id, cancellationToken);
        post.IsPublished = type;
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteStatus(int id ,CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FindAsync(id, cancellationToken);
        post.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
