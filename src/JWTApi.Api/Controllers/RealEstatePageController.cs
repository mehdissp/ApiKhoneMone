using JWTApi.Api.Response;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.RealEstates;
using JWTApi.Domain.Entities;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealEstatePageController : ControllerBase
    {
        private RealEstatesService _realEstatesService;
        private readonly string _encryptionKey;
        public RealEstatePageController(RealEstatesService realEstatesService, IConfiguration configuration)
        {
            _realEstatesService = realEstatesService;
            _encryptionKey = configuration["Encryption:Key"] ??
                   throw new Exception("Encryption key not found");
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
