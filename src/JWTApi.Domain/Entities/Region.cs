using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? ParentId { get; set; }
        public Region? Parent { get; set; } // Nullable
        public Guid? UserId { get; set; }
        public ICollection<Region> Children { get; set; } = new List<Region>(); // لیست 

    }
}
