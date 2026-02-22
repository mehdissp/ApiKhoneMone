using JWTApi.Api.Response;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.RealEstates;
using JWTApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealEstatePageController : ControllerBase
    {
        private RealEstatesService _realEstatesService;
        
        public RealEstatePageController(RealEstatesService realEstatesService)
        {
            _realEstatesService = realEstatesService;
        }
        [HttpGet("GetRandomLastItemRealEstates")]
        public async Task<IActionResult> GetRandomLastItemRealEstates(int tabId,CancellationToken cancellationToken)
        {
            try
            {
       
                var result = await _realEstatesService.GetRandomLastItemRealEstates(tabId,cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet("GetCategoryDtos")]
        public async Task<IActionResult> GetCategoryDtos(int tabId, CancellationToken cancellationToken)
        {
            try
            {

                var result = await _realEstatesService.GetCategoryDtos(tabId, cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
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
    }
}
