using JWTApi.Api.Response;
using JWTApi.Application.DTOs.Posts;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Application.Services.Menus;
using JWTApi.Application.Services.Posts;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private PostsServices _postService;
        private readonly TempImageCache _cache;
        public PostController(PostsServices postsServices, TempImageCache cache )
        {
            _postService = postsServices;
            _cache = cache;
        }
        [HttpGet("GetCategoryPostsDTOs")]
        public async Task<IActionResult> GetCategoryPostsDTOs(CancellationToken cancellationToken)
        {
  
          
                var result = await _postService.GetCategoryPostsDTOs( cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            
        }

        [HttpGet("GetTagsDtosDTOs")]
        public async Task<IActionResult> GetTagsDtosDTOs([FromQuery] int catId,CancellationToken cancellationToken)
        {
            var result = await _postService.GetTagsDtosDTOs(catId,cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }



        [HttpPost("InsertPost")]
        public async Task<IActionResult> InsertPost(PostDtos model, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
                // 1. بررسی تعداد عکس‌ها
                if (model.TempImageCacheIds == null )
                    return BadRequest(new { message = "حداقل یک عکس انتخاب کنید" });

         

                // 2. انتقال عکس‌ها از کش به پوشه اصلی
                var imageUrls = await _cache.MoveToPermanentPost(model.TempImageCacheIds, userId, (int)model.CategoryId);

                if (imageUrls ==null)
                    return BadRequest(new { message = "خطا در انتقال تصاویر" });

                await _postService.InsertPost(model, userId, cancellationToken);


                return Ok(new
                {
                    success = true,
                    message = "مقاله با موفقیت ثبت شد",
                    realEstateId = 1,
                    imageCount = imageUrls
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"خطا در ثبت ملک: {ex.Message}" });
            }
        }


    }
}
