using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Descriptions { get; set; }
        public DateTime CreatedAt { get; set; }
        public  CategoryType CategoryType { get; set; }
        public ICollection<Facilities> Facilities { get; set; } = new List<Facilities>();
        public ICollection<SpecialFeature> SpecialFeature { get; set; } = new List<SpecialFeature>();
    }
}
