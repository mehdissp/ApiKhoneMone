using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.ChatBots;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace JWTApi.Application.Services.ChatBots
{

    public class ChatHub : Hub
    {
        private readonly ChatBotEngine _engine;
        private readonly IRepository<User> _userRepo;

        public ChatHub(ChatBotEngine engine, IRepository<User> userRepo)
        {
            _engine = engine;
            _userRepo = userRepo;
        }

        public async Task SendMessage(string message, Guid userId)
        {
            // بررسی وجود کاربر یا ساخت خودکار
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                user = new User { Name = $"کاربر_{userId}", MobileNumber = "نامشخص" };
                await _userRepo.AddAsync(user);
                await Clients.Caller.SendAsync("NewUserCreated", user.Id);
            }

            // پردازش پیام
            var (response, intent) = await _engine.ProcessMessage(message, userId);

            // ارسال پاسخ لحظه‌ای به کلاینت
            await Clients.Caller.SendAsync("ReceiveMessage", response, intent, DateTime.UtcNow);

            // بروزرسانی آمار برای همه (اختیاری)
            await Clients.All.SendAsync("StatsUpdated", await GetStats());
        }

        private async Task<object> GetStats()
        {
            // آمار را از دیتابیس بگیر
            return new { TotalMessages =0 };
        }
    }
}
