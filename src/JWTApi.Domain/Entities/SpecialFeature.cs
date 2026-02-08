using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class SpecialFeature
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public Guid UserId { get; set; }
        public int CategoryId { get; set; }
        public string? DescriptionRows { get; set; }
        public int? ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category Category { get; set; } = default!;
        public SpecialFeature? Parent { get; set; } // Nullable
        public ICollection<SpecialFeature> Children { get; set; } = new List<SpecialFeature>(); // لیست 

    }
}
