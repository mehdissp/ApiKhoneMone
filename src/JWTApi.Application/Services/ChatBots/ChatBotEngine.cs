using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.ChatBots;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.ChatBots
{
    public class ChatBotEngine
    {
        private readonly AppDbContext _context;
        private readonly ICommandService _commandService;

        public ChatBotEngine(AppDbContext context, ICommandService commandService)
        {
            _context = context;
            _commandService = commandService;
        }

        public async Task<(string response, string intent)> ProcessMessage(string message, Guid userId)
        {
            // 1. بررسی دستورات ویژه
            if (_commandService.IsSpecialCommand(message))
            {
                var result = await _commandService.ExecuteCommand(message, userId);
                return (result, "command");
            }

            // 2. جستجو در داده‌های آموزشی (با TF-IDF ساده)
            var trainingData = await _context.TrainingData.ToListAsync();
            var fallback = "";
            if (!trainingData.Any())
            {
                // اگر داده آموزشی نبود، از کاربر می‌خواد آموزش بده
                 fallback = "📚 من تازه کارم و هنوز یاد نگرفتم! " +
                               "می‌تونید جواب این سوال رو به من یاد بدید:\n" +
                               "دقیقاً جواب این سوال چیه؟ (پیام رو با جواب کامل بفرستید)";
                await SaveHistory(userId, message, fallback, false, "learning");
                return (fallback, "learning");
            }
            var bestMatch = trainingData
                .Select(t => new
                {
                    Data = t,
                    Score = CalculateSimilarity(message, t.Question)
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (bestMatch != null && bestMatch.Score > 0.3)
            {
                // ذخیره در تاریخچه
                await SaveHistory(userId, message, bestMatch.Data.Answer, false, bestMatch.Data.Category);
                return (bestMatch.Data.Answer, bestMatch.Data.Category);
            }
            // اگر داده آموزشی نبود، از کاربر می‌خواد آموزش بده
            fallback = "📚 من تازه کارم و هنوز یاد نگرفتم! " +
                          "می‌تونید جواب این سوال رو به من یاد بدید:\n" +
                          "دقیقاً جواب این سوال چیه؟ (پیام رو با جواب کامل بفرستید)";
            await SaveHistory(userId, message, fallback, false, "learning");
            // 3. پاسخ پیش‌فرض با یادگیری
            //fallback = "متاسفم متوجه نشدم. می‌تونید سوال خودتون رو با کلمات کلیدی مثل قیمت، منطقه، متراژ بپرسید.";
            await SaveHistory(userId, message, fallback, false, "unknown");
            return (fallback, "unknown");
        }

        private double CalculateSimilarity(string input, string question)
        {
            // روش ساده: پیدا کردن کلمات مشترک
            var words1 = input.Split(' ');
            var words2 = question.Split(' ');
            var common = words1.Intersect(words2).Count();
            return (double)common / Math.Max(words1.Length, words2.Length);
        }

        private async Task SaveHistory(Guid userId, string message, string response, bool isCommand, string intent)
        {
            var chatMsg = new ChatMessage
            {
                UserId = userId,
                Message = message,
                Response = response,
                IsCommand = isCommand,
                Intent = intent,
                Timestamp = DateTime.UtcNow
            };
            await _context.ChatMessages.AddAsync(chatMsg);
            await _context.SaveChangesAsync();
        }
    }
}
