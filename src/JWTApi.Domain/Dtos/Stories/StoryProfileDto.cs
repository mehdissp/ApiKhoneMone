using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Stories
{
    public class StoryProfileDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsRealEstate { get; set; }
        public string? UrlImage { get; set; }
        public int? RealEstateId { get; set; }
        public string? TitleReal { get; set; }
        public string? ImgReal { get; set; }
        public string? LinkReal { get; set; }
    }
}
