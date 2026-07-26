using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Regions
{
    public class RegionParentDto
    {
        public int? Id { get; set; }
        public string ParentName { get; set; }
        public string ChildrenNames { get; set; } // فرمت: (جنت,شهران)
        public int? ChildId { get; set; }
    }
}
