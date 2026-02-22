using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class BookMark
    {
        public int Id { get; set; }
        public int RealEstatesId { get; set; }
        public Guid UserId{ get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DescriptionRows { get; set; }

        public RealEstates RealEstates { get; set; } = default!;
    }
}
