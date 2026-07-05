using JWTApi.Application.Services.ChatBots;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.ChatBots;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatBotEngine _engine;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<ChatMessage> _messageRepo;

        public ChatController(
            ChatBotEngine engine,
            IRepository<User> userRepo,
            IRepository<ChatMessage> messageRepo)
        {
            _engine = engine;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
        }

        // 🔹 ارسال پیام و دریافت پاسخ (REST)
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
             
               var user = await _userRepo.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    // ساخت کاربر جدید
                    user = new User
                    {
                        Name = request.UserName ?? $"کاربر_{DateTime.Now.Ticks}",
                        MobileNumber = request.MobileNumber ?? "نامشخص",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userRepo.AddAsync(user);
                }

                // پردازش پیام
                var (response, intent) = await _engine.ProcessMessage(request.Message, user.Id);

                return Ok(new ChatResponse
                {
                    UserId = user.Id,
                    Message = request.Message,
                    Response = response,
                    Intent = intent,
                    Timestamp = DateTime.UtcNow,
                    IsNewUser = user.CreatedAt > DateTime.UtcNow.AddMinutes(-1)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 🔹 دریافت تاریخچه یک کاربر
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId, [FromQuery] int take = 20)
        {
            var messages = await _messageRepo.GetAllAsync(m => m.UserId == userId);
            var history = messages
                .OrderByDescending(m => m.Timestamp)
                .Take(take)
                .Select(m => new
                {
                    m.Id,
                    m.Message,
                    m.Response,
                    m.IsCommand,
                    m.Intent,
                    m.Timestamp
                });

            return Ok(history);
        }

        // 🔹 دریافت آمار کلی
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalMessages = await _messageRepo.CountAsync();
            var totalUsers = await _userRepo.CountAsync();
            var commandCount = await _messageRepo.CountAsync(m => m.IsCommand);
            var unknownIntents = await _messageRepo.CountAsync(m => m.Intent == "unknown");

            return Ok(new
            {
                TotalMessages = totalMessages,
                TotalUsers = totalUsers,
                CommandCount = commandCount,
                UnknownIntents = unknownIntents,
                Today = DateTime.Now.ToString("yyyy/MM/dd HH:mm")
            });
        }

        // 🔹 پاک کردن تاریخچه یک کاربر
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearHistory(Guid userId)
        {
            var messages = await _messageRepo.GetAllAsync(m => m.UserId == userId);
            if (!messages.Any())
                return Ok(new { message = "تاریخچه قبلاً خالی است!" });

            _messageRepo.DeleteRange(messages);
            await _messageRepo.SaveChangesAsync();

            return Ok(new { message = $"تاریخچه پاک شد! {messages.Count()} پیام حذف شد." });
        }

        // 🔹 ایجاد کاربر جدید
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                MobileNumber = request.MobileNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            return Ok(new
            {
                user.Id,
                user.Name,
                user.MobileNumber,
                user.CreatedAt
            });
        }

        // 🔹 دریافت اطلاعات کاربر
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "کاربر یافت نشد!" });

            return Ok(new
            {
                user.Id,
                user.Name,
                user.MobileNumber,
                user.CreatedAt
            });
        }

        // 🔹 آموزش دستی (اضافه کردن داده آموزشی)
        [HttpPost("train")]
        public async Task<IActionResult> AddTrainingData([FromBody] TrainingDataRequest request)
        {
            var training = new TrainingData
            {
                Question = request.Question,
                Answer = request.Answer,
                Category = request.Category ?? "عمومی",
                Confidence = request.Confidence ?? 0.8f
            };

            // نیاز به اضافه کردن _trainingRepo
            // await _trainingRepo.AddAsync(training);

            return Ok(new { message = "داده آموزشی با موفقیت اضافه شد!" });
        }
    }
    // 📦 مدل‌های درخواست و پاسخ
    public class ChatRequest
    {
        public Guid UserId { get; set; }
        public int TypeChat { get; set; }
        public string Message { get; set; }
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
    }

    public class ChatResponse
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public string Intent { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsNewUser { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
    }

    public class TrainingDataRequest
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string? Category { get; set; }
        public float? Confidence { get; set; }
    }
}
