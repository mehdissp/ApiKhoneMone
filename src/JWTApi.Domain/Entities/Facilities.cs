using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Facilities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public Guid UserId { get; set; }
        public int  CategoryId { get; set; }
        public string? DescriptionRows { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category Category { get; set; } = default!;


    }
}
