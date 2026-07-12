using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstatesesApplications
{
    public class RealEstatesesApplicationsDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Regions { get; set; }
        public string RegionName { get; set; }
        public string CategoryName { get; set; }
        public int Code { get; set; }
        public bool IsPaid { get; set; }
        public string MobileNumber { get; set; }
        public string Desc { get; set; }
        public decimal? Budget { get; set; }
        public int? MinCountRoom { get; set; }
        public int? MinConstructionYear { get; set; }
        public int? MaxConstructionYear { get; set; }
        public int? MinSquareMeter { get; set; }
        public int? MaxSquareMeter { get; set; }
        public string FullNameCustomer { get; set; }

        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
    }
}
