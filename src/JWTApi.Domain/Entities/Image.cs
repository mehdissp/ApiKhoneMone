using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public int RealEstateId { get; set; }
        public bool IsBanner { get; set; } = false;
    }
}
