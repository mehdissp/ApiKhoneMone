using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Story
    {
     
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }
        public int RealEstatesId { get; set; }
        public Guid UserId { get; set; }


        public virtual RealEstates? RealEstates { get; set; }
        public virtual User User { get; set; }

        public bool IsExpired => DateTime.UtcNow - CreatedAt > TimeSpan.FromHours(24);
    }
}
