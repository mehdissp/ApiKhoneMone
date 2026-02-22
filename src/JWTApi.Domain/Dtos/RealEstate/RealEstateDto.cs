using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstate
{
    public class RealEstateDto
    {
        public int Id { get; set; }
        public int? ConstructionYear { get; set; }
        public int? CountFloor { get; set; }
        public string Title { get; set; }
        public string AdditionalInformation { get; set; }
        public bool? IsHasElevator { get; set; }
        public bool? IsHasParking { get; set; }
        public bool? IsHasPool { get; set; }
        public bool? IsHasStoreRoom { get; set; }
        public string Name { get; set; } // از جدول Regions (r.Name)
        public string ParentName { get; set; } // q.Name + ' /' + ra.Name
        public string Address { get; set; }
        public int CategoryType { get; set; }
        public int? ImageCount { get; set; }
    }
}
