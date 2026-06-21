using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Stoires
{
    public class StoryRequest
    {
        public string Title { get; set; }
        public string UrlAddress { get; set; }
        public int? RealEstatedId { get; set; }
        public List<string> TempImageCacheIds { get; set; } = new List<string>();

    }
}
