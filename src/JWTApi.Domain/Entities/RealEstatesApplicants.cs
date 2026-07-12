using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RealEstatesApplicants
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Budget { get; set; }
        public int Code { get; set; }
        public int? MinCountRoom { get; set; }
        public int? MinConstructionYear { get; set; }
        public int? MaxConstructionYear { get; set; }
        public int? MinSquareMeter { get; set; }
        public int? MaxSquareMeter { get; set; }
        public int RegionId { get; set; }
        public int CategoryId { get; set; }
        public string Desc { get; set; }

        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Category Category { get; set; } = default!;
        public User User { get; set; } = default!;
        public Region Region { get; set; } = default!;
    }
}
