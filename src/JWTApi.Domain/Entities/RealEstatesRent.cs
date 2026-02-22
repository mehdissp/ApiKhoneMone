using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RealEstatesRent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? DescriptionRows { get; set; }
        public int RoomCount { get; set; }
        public int Floor { get; set; }
        public int CountFloor { get; set; }
        public int CountInFloor { get; set; }
        public int ConstructionYear { get; set; }
        public int SquareMeter { get; set; }
        public long? Deposit { get; set; }
        public string? DepositString { get; set; }
        public long? Rent { get; set; }
        public string? RentString { get; set; }
        public int CategoryId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsHasPool { get; set; } = false;
        public bool IsHasElevator { get; set; } = false;
        public bool IsHasStoreRoom { get; set; } = false;
        public bool IsHasParking { get; set; } = false;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPrivate { get; set; } = false;
        public int RegionId { get; set; }
        public bool Convertible { get; set; } = true;
        public bool IsShowLocation { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public Category Category { get; set; } = default!;
        public User User { get; set; } = default!;
        public Region Region { get; set; } = default!;
        //public ICollection<RealEstates_Facilities> RealEstates_Facilities { get; set; } = new List<RealEstates_Facilities>();
        //public ICollection<RealEstates_SpecialFeature> RealEstates_SpecialFeatures { get; set; } = new List<RealEstates_SpecialFeature>();
        public ICollection<BookMark> BookMark { get; set; } = new List<BookMark>();
        //public ICollection<SearchMatch> Matches { get; set; } = new List<SearchMatch>();
    }
}
