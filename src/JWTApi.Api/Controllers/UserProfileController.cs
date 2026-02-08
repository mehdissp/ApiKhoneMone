using JWTApi.Application.Services;
using JWTApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserService _userService;

        public UserProfileController(AppDbContext context, IWebHostEnvironment env, UserService userService)
        {
            _context = context;
            _env = env;
            _userService = userService;
        }

        public class UploadPhotoDto
        {
            public IFormFile File { get; set; }
        }

        [HttpPost("upload-photo")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDto file, CancellationToken cancellation)
        {
            if (file?.File == null || file.File.Length == 0)
                return BadRequest(new { message = "هیچ فایلی ارسال نشده است." });

            // اعتبارسنجی نوع فایل
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "فرمت فایل مجاز نیست." });
            const int maxFileSize = 3 * 1024 * 1024; // 3 MB
            if (file.File.Length > maxFileSize)
                return BadRequest(new { message = "حجم فایل نباید بیشتر از 3 مگابایت باشد." });

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            string fileName = await _userService.UploadAvatarUser(userId, file.File, cancellation);
            var fileUrl = $"/uploads/users/{fileName}";
            return Ok(new
            {
                message = "عکس با موفقیت آپلود شد.",
                //path = $"/uploads/users/{fileName}",
                fullUrl = $"{Request.Scheme}://{Request.Host}{fileUrl}",
                fileName = file.File.FileName
            });


        }


        // در کنترلر UserProfile این متد را اضافه یا اصلاح کنید
        //[HttpPost("upload-photo")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadTest(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest(new { message = "هیچ فایلی ارسال نشده است." });

        //        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/users");
        //        if (!Directory.Exists(uploadsPath))
        //            Directory.CreateDirectory(uploadsPath);
        //        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        //        var fileName = $"_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        //        var filePath = Path.Combine(uploadsPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // ذخیره در ریشه uploads نه در پوشه users
        //        var fileUrl = $"/uploads/{fileName}";

        //        return Ok(new
        //        {
        //            message = "عکس با موفقیت آپلود شد.",
        //            path = fileUrl,
        //            fullUrl = $"{Request.Scheme}://{Request.Host}{fileUrl}",
        //            physicalPath = filePath
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "خطا در آپلود فایل", error = ex.Message });
        //    }
        //}



    }
}
