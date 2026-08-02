using JWTApi.Api.Response;
using JWTApi.Api.ViewModels.Posts;
using JWTApi.Api.ViewModels.RealEstates;
using JWTApi.Api.ViewModels.Violations;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.Posts;
using JWTApi.Application.Services.RealEstatesApplications;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        private readonly RealEstatesApplicationsServices _realEstatesApplicationsServices;
        private PostsServices _postService;
        public RealEstatePageController(RealEstatesService realEstatesService, 
            IConfiguration configuration, TempImageCache cache, RealEstatesApplicationsServices realEstatesApplicationsServices, PostsServices postsServices

            )
        {
            _realEstatesService = realEstatesService;
            _encryptionKey = configuration["Encryption:Key"] ??
                   throw new Exception("Encryption key not found");
            _cache = cache;
            _realEstatesApplicationsServices= realEstatesApplicationsServices;
            _postService= postsServices;

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
        //    [HttpGet("GetRandomLastItemRealEstatesWithCategoryAsync")]
        //    [PublicEndpoint]
        //    public async Task<IActionResult> GetLastItems(
        //int tabId,
        //int pageNumber = 1,
        //int pageSize = 10)
        //    {
        //        var result = await _realEstatesService.GetRandomLastItemRealEstatesWithCategoryAsync(
        //            tabId,
        //            pageNumber,
        //            pageSize);

        //        return ResponseApi.Ok(result).ToHttpResponse();
        //    }

        // RealEstateController.cs
        [HttpGet("GetRandomLastItemRealEstatesWithCategoryAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetLastItems([FromQuery] FilterRealEstateDto filter)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            //var result = await _realEstatesService.GetRandomLastItemRealEstatesWithCategoryAsync(filter.TabId,filter.PageNumber,filter.PageSize);
            var result=await _realEstatesService.GetFilteredRealEstatesWithCategoryFilterAsync(filter, userId);
            return ResponseApi.Ok(result).ToHttpResponse();
        }
        [HttpGet("GetRealStateMap")]
        public async Task<IActionResult> GetRealStateMap(
int tabId,
int pageNumber = 1,
int pageSize = 10)
        {
            var result = await _realEstatesService.GetRealStateMap(
                tabId,
                pageNumber,
                pageSize);

            return ResponseApi.Ok(result).ToHttpResponse();
        }

        
        [HttpGet("GetRealEstateDetails")]
        public async Task<IActionResult> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _realEstatesService.GetRealEstateDetails(id, userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("GetRealEstateDetailsForDemo")]
        [Authorize]
        public async Task<IActionResult> GetRealEstateDetailsForDemo(int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleName = User.Claims.FirstOrDefault(c => c.Type == "roleName")?.Value;
            //var check = await _realEstatesService.CheckAccessToRealEstate(id, userId, roleName, cancellationToken);
            //if (check)
            //{
                var result = await _realEstatesService.GetRealEstateDetails(id, userId, cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            //}
            //return ResponseApi.Error("دسترسی ندارید به این صفحه").ToHttpResponse();

        }
        [HttpGet("GetRealEstateDetailsForEdit")]
        [Authorize]
        public async Task<IActionResult> GetRealEstateDetailsForEdit(int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleName = User.Claims.FirstOrDefault(c => c.Type == "roleName")?.Value;
            var check = await _realEstatesService.CheckAccessToRealEstate(id, userId, roleName, cancellationToken);
            if (check)
            {

                var result = await _realEstatesService.GetRealEstateDetailsForEdit(id, cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            return ResponseApi.Error("دسترسی ندارید به این صفحه").ToHttpResponse();
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

        [HttpGet("GetRegions")]
        public async Task<IActionResult> GetRegions( CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetRegions( cancellationToken);

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
                var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
                // 1. بررسی تعداد عکس‌ها
                if (model.TempImageCacheIds == null || model.TempImageCacheIds.Count == 0)
                    return BadRequest(new { message = "حداقل یک عکس انتخاب کنید" });

                if (model.TempImageCacheIds.Count > 12)
                    return BadRequest(new { message = "حداکثر 12 عکس مجاز است" });

                // 2. انتقال عکس‌ها از کش به پوشه اصلی
                var imageUrls = await _cache.MoveToPermanent(model.TempImageCacheIds, userId,model.CategoryTypeId);

                if (imageUrls.Count == 0)
                    return BadRequest(new { message = "خطا در انتقال تصاویر" });
       
                await _realEstatesService.InsertRealEstate(model, userId, roleId, imageUrls, cancellationToken);
               

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

        [HttpGet("GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus(int id,CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

            var result = await _realEstatesService.GetPaymentStatus(id, roleId, userId);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("ToggleBookMark")]
        public async Task<IActionResult> ToggleBookMark([FromBody] int realEstateId,CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _realEstatesService.ToggleBookMark(userId, realEstateId, cancellationToken);
            return Ok();
        }
        [HttpPost("InsertViolations")]
        public async Task<IActionResult> InsertViolations([FromBody] ViolationRequest violationRequest, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _realEstatesService.InsertViolations(userId, violationRequest.Id, violationRequest.Desc, violationRequest.ErrorType, cancellationToken);
            return ResponseApi.Ok().ToHttpResponse();
        }

        [HttpGet("violation-types")]
        public async Task<IActionResult> GetAllViolationTypes()
        {
            var result = Enum.GetValues(typeof(ViolationTypeEnum))
                             .Cast<ViolationTypeEnum>()
                             .Select(e => new
                             {
                                 Id = (int)e,
                                 Name = e.ToPersianString().ToString()
                             })
                             .ToList();
            return ResponseApi.Ok(result).ToHttpResponse();
           
        }

        [HttpGet("GetUserForSite")]
        public async Task<IActionResult> GetUserForSite(string userId,CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetUserForSite(userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpGet("GetRandomLastItemRealEstatesWithUser")]
        public async Task<IActionResult> GetRandomLastItemRealEstatesWithUser(
 string userId,
 int pageNumber = 1,
 int pageSize = 10)
        {
            var result = await _realEstatesService.GetRandomLastItemRealEstatesWithUser(
                userId,
                pageNumber,
                pageSize);

            return ResponseApi.Ok(result).ToHttpResponse();
        }
        [HttpGet("GetRealEstateBookMark")]
        public async Task<IActionResult> GetRealEstateBookMark(
int pageNumber = 1,
int pageSize = 10)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _realEstatesService.GetRealEstateBookMark(
                userId,
                pageNumber,
                pageSize);

            return ResponseApi.Ok(result).ToHttpResponse();
        }


        [HttpGet("GetIndependentAgent")]
        public async Task<IActionResult> GetIndependentAgent( CancellationToken cancellationToken)
        {


            var result = await _realEstatesService.GetIndependentAgent( cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpGet("RealEstatesesApplicationsDtosAsync")]
        public async Task<IActionResult> RealEstatesesApplicationsDtosAsync(
             string? searchTrem,
            int pageNumber ,
            int pageSize ,
            CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;


            var result = await _realEstatesApplicationsServices.RealEstatesesApplicationsDtosAsync(
                userId,
                  searchTrem,
                pageNumber,
                pageSize,
                cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();
        }

        [HttpPost("GetRealEstatesesApplicationsDetails")]
        public async Task<IActionResult> GetRealEstatesesApplicationsDetails([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
         var result  = await _realEstatesApplicationsServices.GetRealEstatesesApplicationsDetails(id,userId, cancellationToken);
            return ResponseApi.Ok(result).ToHttpResponse();
        }
        [HttpGet("GetPaymentStatusRealApp")]
        public async Task<IActionResult> GetPaymentStatusRealApp(int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

            var result = await _realEstatesApplicationsServices.GetPaymentStatus(id, roleId, userId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

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

        [HttpPost("GetPostCategoryDtoAdmin")]
        [Authorize]

        public async Task<IActionResult> GetPostCategoryDtoAdmin([FromBody] PostsRequestViewModel postsRequestViewModel, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _postService.GetPostCategoryDtoAdmin(postsRequestViewModel.PageNumber, postsRequestViewModel.PageSize, postsRequestViewModel.SearchStream, postsRequestViewModel.CategoryId, postsRequestViewModel.IsPublished);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("GetRegionsWithChildrenLinq")]
        [PublicEndpoint]
        public async Task<IActionResult> GetRegionsWithChildrenLinq([FromBody] int id)
        {


            var result = await _realEstatesService.GetRegionsWithChildrenLinq(id);

            return ResponseApi.Ok(result).ToHttpResponse();

        }
        [HttpPost("UpdateViewCount")]
        [PublicEndpoint]
        [AllowedOrigins]
        public async Task<IActionResult> UpdateViewCount([FromBody] int id, CancellationToken cancellationToken)
        {


            await _realEstatesService.UpdateViewCount(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("GetRandomLastItemRealEstatesWithSimpleAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetRandomLastItemRealEstatesWithSimpleAsync([FromBody] int id)
        {


            var result = await _realEstatesService.GetRandomLastItemRealEstatesWithSimpleAsync(id);

            return ResponseApi.Ok(result).ToHttpResponse();

        }

        [HttpPost("GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync")]
        [PublicEndpoint]
        public async Task<IActionResult> GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync()
        {


            var result = await _realEstatesService.GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync();

            return ResponseApi.Ok(result).ToHttpResponse();

        }



    }
}
