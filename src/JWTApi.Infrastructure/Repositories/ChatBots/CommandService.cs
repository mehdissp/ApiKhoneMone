using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.ChatBots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.ChatBots
{


    public class CommandService : ICommandService
    {
        private readonly IRepository<ChatMessage> _messageRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<TrainingData> _trainingRepo; // ⭐ اضافه شد

        public CommandService(IRepository<ChatMessage> messageRepo, IRepository<User> userRepo, IRepository<TrainingData> trainingRepo) // ⭐ پارامتر جدید
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _trainingRepo = trainingRepo;
        }

        public bool IsSpecialCommand(string message)
        {
            var commands = new[] { "/newuser", "/history", "/clear", "/stats", "/teach", "/learn" };
            return commands.Any(message.StartsWith);
        }

        public async Task<string> ExecuteCommand(string message, Guid userId)
        {
            var parts = message.Split(' ');
            var command = parts[0];
            var training = new TrainingData();
            switch (command)
            {
                // در CommandService.cs

                case "/teach":
                    // آموزش مستقیم به ربات
                    // فرمت: /teach سوال | جواب
                    var fullText = message.Substring(6).Trim(); // حذف /teach
                     parts = fullText.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 2)
                    {
                        return "❌ فرمت اشتباه! استفاده کنید:\n" +
                               "/teach سوال | جواب\n\n" +
                               "مثال: /teach قیمت منطقه ۸ | قیمت منطقه ۸ حدود ۵۰-۷۵ میلیون تومانه";
                    }

                    var question = parts[0].Trim();
                    var answer = parts[1].Trim();

                    // بررسی تکراری نبودن
                    var exists = await _trainingRepo.AnyAsync(t =>
                        t.Question.ToLower() == question.ToLower());

                    if (exists)
                    {
                        return $"⚠️ این سوال قبلاً در دیتابیس وجود دارد!\n" +
                               $"سوال: {question}\n" +
                               $"برای ویرایش از /learn استفاده کنید.";
                    }

                     training = new TrainingData
                    {
                        Question = question,
                        Answer = answer,
                        Category = "آموزش کاربر",
                        Confidence = 0.9f
                    };
                    await _trainingRepo.AddAsync(training);

                    return $"✅ سوال جدید با موفقیت به دیتابیس اضافه شد!\n" +
                           $"📝 سوال: {question}\n" +
                           $"📝 جواب: {answer}";
                case "/newuser":
                    // خودکار کاربر جدید می‌سازه
                    var newUser = new User { Name = parts[1] ?? "کاربر جدید", MobileNumber = parts[2] ?? "نامشخص" };
                    await _userRepo.AddAsync(newUser);
                    return $"✅ کاربر جدید با نام {newUser.Name} ساخته شد!";

                case "/history":
                    // نمایش تاریخچه
                    var history = await _messageRepo.GetAllAsync(m => m.UserId == userId);
                    return string.Join("\n", history.Select(m => $"{m.Timestamp}: {m.Message} -> {m.Response}"));

                case "/clear":
                    // پاک کردن حافظه
                    var messages = await _messageRepo.GetAllAsync(m => m.UserId == userId);
                    foreach (var msg in messages)
                        await _messageRepo.DeleteAsync(msg);
                    return "🧹 حافظه پاک شد!";

                case "/stats":
                    // آمار
                    var stats = await _messageRepo.GetAllAsync();
                    return $"📊 آمار:\nکل پیام‌ها: {stats.Count()}\nکاربران: {await _userRepo.CountAsync()}\nدستورات استفاده شده: {stats.Count(m => m.IsCommand)}";

                case "/learn":
                    // یادگیری از مکالمه
                    var lastMsg = await _messageRepo.GetLastAsync(m => m.UserId == userId);
                    if (lastMsg != null)
                    {
                         training = new TrainingData
                        {
                            Question = lastMsg.Message,
                            Answer = lastMsg.Response,
                            Category = "یادگیری جدید"
                        };
                        await _trainingRepo.AddAsync(training);
                        return "🧠 از مکالمه یاد گرفتم!";
                    }
                    return "مکالمه‌ای برای یادگیری پیدا نشد";

                default:
                    return "دستور نامعتبر";
            }
        }
    }
}
