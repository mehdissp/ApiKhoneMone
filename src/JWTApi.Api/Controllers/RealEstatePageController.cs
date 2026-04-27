using JWTApi.Api.Response;
using JWTApi.Api.ViewModels.RealEstates;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Domain.Entities;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealEstatePageController : ControllerBase
    {
        private RealEstatesService _realEstatesService;
        private readonly string _encryptionKey;
        private readonly TempImageCache _cache;
        public RealEstatePageController(RealEstatesService realEstatesService, IConfiguration configuration, TempImageCache cache)
        {
            _realEstatesService = realEstatesService;
            _encryptionKey = configuration["Encryption:Key"] ??
                   throw new Exception("Encryption key not found");
            _cache = cache;
        }
        [HttpGet("GetRandomLastItemRealEstates")]
        public async Task<IActionResult> GetRandomLastItemRealEstates(int tabId,CancellationToken cancellationToken)
        {

       
                var result = await _realEstatesService.GetRandomLastItemRealEstates(tabId,cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
         
        }

        [HttpGet("GetCategoryDtos")]
        public async Task<IActionResult> GetCategoryDtos(int tabId, CancellationToken cancellationToken)
        {

                var result = await _realEstatesService.GetCategoryDtos(tabId, cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
       
        }
        [HttpGet("GetRandomLastItemRealEstatesWithCategoryAsync")]
        public async Task<IActionResult> GetLastItems(
    int tabId,
    int pageNumber = 1,
    int pageSize = 10)
        {
            var result = await _realEstatesService.GetRandomLastItemRealEstatesWithCategoryAsync(
                tabId,
                pageNumber,
                pageSize);

            return ResponseApi.Ok(result).ToHttpResponse();
        }
        [HttpGet("GetRealEstateDetails")]
        public async Task<IActionResult> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetRealEstateDetails(id, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetRealEstatePanel")]
        [Authorize]
        public async Task<IActionResult> GetRealEstatePanel( CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var result = await _realEstatesService.GetRealEstatePanel(userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetFacilities")]
        public async Task<IActionResult> GetFacilitiesDtos(int id, CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetFacilitiesDtos(id, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetRegionsWithChildFlagAsync")]
        public async Task<IActionResult> GetRegionsWithChildFlagAsync(int? id, CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetRegionsWithChildFlagAsync(id, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpPost("UploadTempImage")]
        public async Task<IActionResult> UploadTempImage(IFormFile image)
        {
            try
            {
                // بررسی حجم (5 مگابایت)
                if (image.Length > 10 * 1024 * 1024)
                    return BadRequest(new { message = "حجم فایل بیشتر از 10 مگابایت است" });

                // ذخیره در کش
                var cacheId = await _cache.SaveToCache(image);

                return Ok(new
                {
                    success = true,
                    cacheId = cacheId
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

        [HttpPost("InsertRealEstate")]
        public async Task<IActionResult> InsertRealEstate(RealEstateRequest model, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                // 1. بررسی تعداد عکس‌ها
                if (model.TempImageCacheIds == null || model.TempImageCacheIds.Count == 0)
                    return BadRequest(new { message = "حداقل یک عکس انتخاب کنید" });

                if (model.TempImageCacheIds.Count > 12)
                    return BadRequest(new { message = "حداکثر 12 عکس مجاز است" });

                // 2. انتقال عکس‌ها از کش به پوشه اصلی
                var imageUrls = await _cache.MoveToPermanent(model.TempImageCacheIds, userId,model.CategoryTypeId);

                if (imageUrls.Count == 0)
                    return BadRequest(new { message = "خطا در انتقال تصاویر" });
       
                await _realEstatesService.InsertRealEstate(model, userId, imageUrls, cancellationToken);


                return Ok(new
                {
                    success = true,
                    message = "ملک با موفقیت ثبت شد",
                    realEstateId = 1,
                    imageCount = imageUrls.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"خطا در ثبت ملک: {ex.Message}" });
            }
        }

        //[HttpGet("GetRealEstateDetails")]
        //public async Task<IActionResult> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var result = await _realEstatesService.GetRealEstateDetails(id, cancellationToken);
        //        var jsonData = JsonSerializer.Serialize(result);

        //        // رمزگذاری
        //        var encrypted = AesEncryption.Encrypt(jsonData, _encryptionKey, out string iv);

        //        return Ok(new EncryptedResponse
        //        {
        //            Data = Convert.ToBase64String(encrypted),
        //            Iv = iv
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}
        //public class EncryptedResponse
        //{
        //    public string Data { get; set; }
        //    public string Iv { get; set; }
        //}

    }
}
