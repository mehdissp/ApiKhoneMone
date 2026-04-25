using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Regions
{
    public class RegionDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int CountChild { get; set; } // 1 اگر فرزند دارد، در غیر این صورت 0
        public bool HasRegion { get; set; }
    }
}
