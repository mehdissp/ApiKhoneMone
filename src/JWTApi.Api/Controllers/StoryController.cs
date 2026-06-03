using JWTApi.Api.Response;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Application.DTOs.Stoires;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Application.Services.Stories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly StoryService _storyService;
    private readonly TempImageCache _cache;
    public StoryController(StoryService storyService, TempImageCache cache)
    {
        _storyService = storyService;
        _cache = cache;
    }

    [HttpPost("InsertStory")]
    public async Task<IActionResult> InsertStory(StoryRequest model, CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
        // 1. بررسی تعداد عکس‌ها
        if (model.TempImageCacheIds == null || model.TempImageCacheIds.Count == 0)
            return BadRequest(new { message = "حداقل یک عکس انتخاب کنید" });

        if (model.TempImageCacheIds.Count > 12)
            return BadRequest(new { message = "حداکثر 12 عکس مجاز است" });
        // 2. انتقال عکس‌ها از کش به پوشه اصلی
        var imageUrls = await _cache.MoveToPermanentStory(model.TempImageCacheIds, userId);

        if (imageUrls.Count == 0)
            return BadRequest(new { message = "خطا در انتقال تصاویر" });
        model.UrlAddress = imageUrls.First().Url;
        var result = await _storyService.CreateStory(model,userId);


        return ResponseApi.Ok(result).ToHttpResponse();
    }

    [HttpPost("DeleteStory")]
    public async Task<IActionResult> DeleteStory([FromQuery] int id, CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var roleName = User.Claims.FirstOrDefault(c => c.Type == "roleName")?.Value;

        var result = await _storyService.DeleteStory(id, userId, roleName);

        return ResponseApi.Ok(result).ToHttpResponse();
    }

    [HttpGet("StoryProfile")]
    public async Task<IActionResult> StoryProfile( CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
  

        var result = await _storyService.StoryProfileDtosAsync( userId, cancellationToken);

        return ResponseApi.Ok(result).ToHttpResponse();
    }

}

