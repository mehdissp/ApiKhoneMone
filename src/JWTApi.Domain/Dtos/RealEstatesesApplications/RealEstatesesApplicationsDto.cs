using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstatesesApplications
{
    public class RealEstatesesApplicationsDto
    {
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Regions { get; set; }
        public string RegionName { get; set; }
        public string CategoryName { get; set; }
        public int Code { get; set; }
        public bool IsPaid { get; set; }
        public string MobileNumber { get; set; }
        

        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
    }
}
