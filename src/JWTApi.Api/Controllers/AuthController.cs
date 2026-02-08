using JWTApi.Api.Response;
using JWTApi.Application.DTOs;
using JWTApi.Application.Services;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Imaging;

using JWTApi.Api.ViewModels;
using JWTApi.Domain.Entities;

namespace JWTApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly BaleService _baleService;
        private readonly IMemoryCache _memoryCache;
        private static readonly Random Rand = new();
        public AuthController(AuthService authService , IMemoryCache memoryCache, BaleService baleService)
        {
            _memoryCache = memoryCache;
            _authService = authService;
            _baleService = baleService;
        }

        [HttpGet]
        [Authorize]
        [RequirePermission("Get")]
        public IActionResult Get(int id) => Ok(new[] { "user1", "user2" });
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            var (success, message) = await _authService.RegisterAsync(dto, cancellationToken);
            //return success ? Ok(message) : BadRequest(message);
            return success
   ? ResponseApi.Ok(message).ToHttpResponse()
   : Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var (success, token, refresh,expireToken) = await _authService.LoginAsync(dto, ip, cancellationToken);
            var result = new { token, refreshToken = refresh,ExpireToken= expireToken };

            //await _baleService.SendWelcomeMessage(dto.Username, ip);
            return success
        ? ResponseApi.Ok(result).ToHttpResponse()
        : Unauthorized();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDto dto, CancellationToken cancellationToken)
        {
            var (success, token, refresh, expireToken) = await _authService.RefreshAsync(dto, cancellationToken);
            //   return success ? Ok(new { token, refreshToken = refresh }) : Unauthorized();
            var result = new { token, refreshToken = refresh, ExpireToken = expireToken };
            return success
        ? ResponseApi.Ok(result).ToHttpResponse()
        : Unauthorized();
        }
   

    [HttpGet("profile")]
    [Authorize] // فقط کاربر با توکن معتبر می‌تواند به این دسترسی داشته باشد
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        // گرفتن UserId از Claim توکن
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleName = User.Claims.FirstOrDefault(c => c.Type == "roleName")?.Value;
            var test = User.Claims;
        if (userId == null)
            return Unauthorized();

        var user = await _authService.GetUserProfile(userId, cancellationToken);
            user.RoleName = roleName;
        if (user == null)
           return NotFound();

            return
              ResponseApi.Ok(user).ToHttpResponse();
             
        }

        [HttpGet("GetMenus")]
        [Authorize] // فقط کاربر با توکن معتبر می‌تواند به این دسترسی داشته باشد
        public async Task<IActionResult> GetMenus(CancellationToken cancellationToken)
        {
            // گرفتن UserId از Claim توکن
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var test = User.Claims;
            if (userId == null)
                return Unauthorized();

            var user = await _authService.GetUserMenuPermissionsAsync(userId, cancellationToken);
            if (user == null)
                return NotFound();

            return
              ResponseApi.Ok(user).ToHttpResponse();

        }

        [HttpGet("GetMenusForUi")]
        [Authorize] // فقط کاربر با توکن معتبر می‌تواند به این دسترسی داشته باشد
        public async Task<IActionResult> GetMenusForUi(CancellationToken cancellationToken)
        {
            // گرفتن UserId از Claim توکن
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

            
            var test = User.Claims;
            if (userId == null)
                return Unauthorized();

            var user = await _authService.GetUserMenuPermissionsForUiAsync(userId, roleId, cancellationToken);
            if (user == null)
                return NotFound();

            return
              ResponseApi.Ok(user).ToHttpResponse();

        }
       
        [HttpGet("captcha")]
        public IActionResult GetCaptcha()
        {
            var randomNumber = Rand.Next(1000, 9999).ToString();
            var captchaId = Guid.NewGuid().ToString();

            _memoryCache.Set(captchaId, randomNumber, TimeSpan.FromMinutes(2));

            int width = 180;
            int height = 60;
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // کمی نویز پس‌زمینه (گرادیان ساده)
            for (int i = 0; i < 500; i++)
            {
                var color = Color.FromArgb(235, 200 + Rand.Next(0, 15), 240 + Rand.Next(0, 15), 200 + Rand.Next(0, 15));
                g.DrawLine(new Pen(color), i, 0, i, height);
            }

            // کشیدن خطوط تصادفی مورب
            for (int i = 0; i < 550; i++)
            {
                var pen = new Pen(Color.FromArgb(Rand.Next(80, 150), Rand.Next(0, 255), Rand.Next(0, 255), Rand.Next(0, 255)), 1.5f);
                int x1 = Rand.Next(width);
                int y1 = Rand.Next(height);
                int x2 = Rand.Next(width);
                int y2 = Rand.Next(height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }

            // کشیدن اعداد با چرخش و رنگ تصادفی
            using var font = new System.Drawing.Font("Arial", 28, FontStyle.Bold);
            int startX = 25;

            foreach (char c in randomNumber)
            {
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddString(c.ToString(), font.FontFamily, (int)FontStyle.Bold, 36, new Point(0, 0), StringFormat.GenericDefault);

                float angle = Rand.Next(-15, 15);
                g.TranslateTransform(startX, Rand.Next(5, 15));
                g.RotateTransform(angle);

                using var brush = new SolidBrush(Color.FromArgb(
                    255,
                    Rand.Next(0, 100),
                    Rand.Next(0, 100),
                    Rand.Next(0, 100)
                ));
                g.FillPath(brush, path);

                g.ResetTransform();
                startX += 30;
            }

            // نقاط نویز رنگی
            for (int i = 0; i < 900; i++)
            {
                int x = Rand.Next(width);
                int y = Rand.Next(height);
                bmp.SetPixel(x, y, Color.FromArgb(Rand.Next(150, 255), Rand.Next(256), Rand.Next(256), Rand.Next(256)));
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var dataUrl = $"data:image/png;base64,{base64}";

            return Ok(new { captchaId, image = dataUrl });
        }


        [HttpPost("verify-captcha")]
        public IActionResult VerifyCaptcha([FromBody] VerifyCaptchaModel model)
        {
            // گرفتن مقدار کپچا از حافظه
            if (!_memoryCache.TryGetValue(model.CaptchaId, out string correct))
            {
                return BadRequest(new { error = "captcha_invalid" });
            }

            // مقایسه مقدار وارد شده توسط کاربر با کپچا
            if (!string.Equals(correct, model.UserInput, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "captcha_invalid" });
            }

            // اگر درست بود
            return Ok(new { valid = true });
        }


    }
}
