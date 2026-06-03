using JWTApi.Domain.Shared;
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

        public string? Desc { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }
        public int? RealEstatesId { get; set; }
        public Guid UserId { get; set; }
        public StoryStatusEnum? Status { get; set; }


        public virtual RealEstates? RealEstates { get; set; }


        public bool IsExpired => DateTime.UtcNow - CreatedAt > TimeSpan.FromHours(24);

        public void Create(string? content,string? imgpath,int? realEstateId,string userId, StoryStatusEnum storyStatusEnum )
        {
            Desc = content; 
            ImagePath=imgpath;
            RealEstatesId=realEstateId;
            UserId = Guid.Parse(userId);
            Status = storyStatusEnum;
        }
    }
}
