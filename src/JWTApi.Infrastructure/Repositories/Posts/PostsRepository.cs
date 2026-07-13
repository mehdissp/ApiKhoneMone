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

namespace JWTApi.Infrastructure.Repositories.Posts
{
    public class PostsRepository:IPostsRepository
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


        public async Task<List<TagsDtos>> GetTagsDtosDTOs(int catId,CancellationToken cancellationToken)
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
        
        public async Task InsertPost(Post post,CancellationToken cancellationToken)
        {
            await _context.Posts.AddAsync(post, cancellationToken);

        }
        public async Task InsertTags(int[] id,int postId)
        {
         //   tagp
        }

    }
}
