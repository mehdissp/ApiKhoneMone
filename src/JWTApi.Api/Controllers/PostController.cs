using JWTApi.Api.Response;
using JWTApi.Api.ViewModels.Posts;
using JWTApi.Api.ViewModels.RealEstates;
using JWTApi.Application.DTOs.Posts;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Application.Services.Menus;
using JWTApi.Application.Services.Posts;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Domain.Entities;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

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

                if (imageUrls ==null )
                    return BadRequest(new { message = "خطا در انتقال تصاویر" });
                model.ImageUrl = imageUrls.FileName;

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
                return BadRequest(new { message = $"خطا در ثبت مقاله: {ex.Message}" });
            }
        }

        [HttpPost("UploadTempImagePost")]
        public async Task<IActionResult> UploadTempImagePost(IFormFile image)
        {
            try
            {
                // بررسی حجم (5 مگابایت)
                if (image.Length > 10 * 1024 * 1024)
                    return BadRequest(new { message = "حجم فایل بیشتر از 10 مگابایت است" });

                // ذخیره در کش
                var (cacheId, fileName) = await _cache.SaveToPoste(image);
                var imageUrls = await _cache.MoveToPermanentPostForContent(cacheId);
                

                return Ok(new
                {
                    success = true,
                    cacheId = cacheId,
                    fileName = imageUrls.FileName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("ClearTempImage")]
        public IActionResult ClearTempImage([FromBody] ClearTempImageRequest request)
        {
            _cache.ClearCache(request.CacheId);
            return Ok(new { success = true, message = "تصویر حذف شد" });
        }

        [HttpPost("getPostCategoryDto")]
        [PublicEndpoint]
        public async Task<IActionResult> getPostCategoryDto([FromBody] PostsRequestViewModel  postsRequestViewModel,CancellationToken cancellationToken)
        {


            var result = await _postService.getPostCategoryDto(postsRequestViewModel.CategoryId, postsRequestViewModel.SearchStream, postsRequestViewModel.PageSize, postsRequestViewModel.PageNumber, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpGet("GetDetailsDtosAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetDetailsDtosAsync([FromQuery] int id, CancellationToken cancellationToken)
        {


            var result = await _postService.GetDetailsDtosAsync(id,cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetTopViewedPostsAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetTopViewedPostsAsync( CancellationToken cancellationToken)
        {


            var result = await _postService.GetTopViewedPostsAsync( cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetTopNewPostsAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetTopNewPostsAsync(CancellationToken cancellationToken)
        {


            var result = await _postService.GetTopNewPostsAsync(cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("UpdateViewCount")]
        [PublicEndpoint]
        [AllowedOrigins("http://localhost:3000", "https://localhost:3000")]
        public async Task<IActionResult> UpdateViewCount([FromBody] int id, CancellationToken cancellationToken)
        {


          await _postService.UpdateViewCount(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("GetPostCategoryDtoAdmin")]
        [Authorize]

        public async Task<IActionResult> GetPostCategoryDtoAdmin([FromBody] PostsRequestViewModel postsRequestViewModel, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _postService.GetPostCategoryDtoAdmin(postsRequestViewModel.PageNumber,postsRequestViewModel.PageSize,postsRequestViewModel.SearchStream,postsRequestViewModel.CategoryId,postsRequestViewModel.IsPublished);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("DeleteStatus")]
    
    
        public async Task<IActionResult> DeleteStatus([FromBody] int id, CancellationToken cancellationToken)
        {


            await _postService.DeleteStatus(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] PostUpdateStatusViewModel postUpdateStatusViewModel, CancellationToken cancellationToken)
        {


            await _postService.UpdateStatus(postUpdateStatusViewModel.Id, postUpdateStatusViewModel.Type, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }




    }
}
