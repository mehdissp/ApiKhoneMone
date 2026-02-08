using JWTApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{

    public class BaleRepository : IBaleRepository
    {
        private readonly string _botToken;
        private readonly HttpClient _httpClient;
    

        public BaleRepository(IConfiguration configuration, HttpClient httpClient)
        {
            _botToken = configuration["Bale:BotToken"]; // توکن از appsettings.json
            _httpClient = httpClient;
    
        }

        public async Task<string> GetUserChatId()
        {
            var url = $"https://tapi.bale.ai/bot{_botToken}/getUpdates";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var updates = JsonSerializer.Deserialize<BaleUpdatesResponse>(content);
                foreach (var update in updates.Result)
                {
                    var chatId = update.Message.Chat.Id.ToString();
                    var username = update.Message.Chat.Username;
              
                }
            }

            return content;
        }

        public class BaleUpdatesResponse
        {
            [JsonPropertyName("ok")]
            public bool Ok { get; set; }

            [JsonPropertyName("result")]
            public List<BaleUpdate> Result { get; set; }
        }

        public class BaleUpdate
        {
            [JsonPropertyName("message")]
            public BaleMessage Message { get; set; }
        }

        public class BaleMessage
        {
            [JsonPropertyName("chat")]
            public BaleChat Chat { get; set; }
        }

        public class BaleChat
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("username")]
            public string Username { get; set; }
        }

      
    }
}
