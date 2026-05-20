using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.RealEstate
{
    public class RealEstateDetailsEdit
    {
        public int Id { get; set; }
        public int CategoryType { get; set; }
        public int? ConstructionYear { get; set; }
        public int? CountFloor { get; set; }
        public int? Floor { get; set; }
        public string Title { get; set; }
        public int? Rooms { get; set; }
        public string AdditionalInformation { get; set; }
        public bool IsHasElevator { get; set; }
        public bool IsHasParking { get; set; }
        public bool IsHasPool { get; set; }
        public bool IsHasStoreRoom { get; set; }
        public bool IsHasLoan { get; set; }
        public string RegionName { get; set; }
        public string ParentName { get; set; }
        public string Address { get; set; }
        public int ImageCount { get; set; }
        public decimal? Price { get; set; }

        public decimal? PriceMeter { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Deposit { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Facilities { get; set; }
     
        public string[] Images { get; set; }
        public decimal? lng { get; set; }
        public decimal? lat { get; set; }
        public bool ShowExactLocation { get; set; }
        public string DescriptionRows { get; set; }
        public int RegionId { get; set; }

    }
 

}
