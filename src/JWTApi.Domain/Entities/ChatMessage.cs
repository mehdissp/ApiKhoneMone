using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public bool IsCommand { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Intent { get; set; } // مشاوره، قیمت‌یابی، بازدید، و...
        
    }
}
