using JWTApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
  public  class BaleService
    {
        private readonly IBaleRepository _baleRepository;
        public BaleService(IBaleRepository baleRepository)
        {
            _baleRepository = baleRepository;
        }
        public async Task SendWelcomeMessage(string username, string ip)
        {
            try
            {
                var welcomeMessage = $"👋 سلام {username}!\n" +
                                   $"به سامانه ما خوش آمدید.\n" +
                                   $"ورود شما از آی‌پی: {ip} ثبت شد.\n" +
                                   $"زمان: {DateTime.Now:yyyy/MM/dd HH:mm}";

               // await _baleRepository.SendMessageToUser(username, welcomeMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
