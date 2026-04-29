using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RealEstates
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
        public long? Price { get; set; }
        public string? PriceString { get; set; }
        public long? Deposit { get; set; }
        public string? DepositString { get; set; }
        public long? Rent { get; set; }
        public string? RentString { get; set; }
        public long? PricePerMeter { get; set; }
        public string? PricePerMeterString { get; set; }
        public int CategoryId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool IsHasPool { get; set; } = false;
        public bool IsHasElevator { get; set; } = false;
        public bool IsHasStoreRoom { get; set; } = false;
        public bool IsHasParking { get; set; } = false;
        public bool IsHaLoan { get; set; } = false;
        public bool IsRenovated { get; set; } = false;
        public bool IsNegotiatedPrice { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPrivate { get; set; } = false;
        public int RegionId { get; set; }
        public string Address { get; set; }
        public bool IsShowLocation { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public RealEstateStatusEnum  Status { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public Category Category { get; set; } = default!;
        public User User { get; set; } = default!;
        public Region Region { get; set; } = default!;
        //public ICollection<RealEstates_Facilities> RealEstates_Facilities { get; set; } = new List<RealEstates_Facilities>();
        //public ICollection<RealEstates_SpecialFeature> RealEstates_SpecialFeatures { get; set; } = new List<RealEstates_SpecialFeature>();
        public ICollection<BookMark> BookMark { get; set; } = new List<BookMark>();
        //public ICollection<SearchMatch> Matches { get; set; } = new List<SearchMatch>();

        public void create(string title,string desc,int roomCount,int floor
            ,int countFloor,int countInFloor,int constructionYear,int squareMeter,long price,long? deposit
            ,long? rent,int categoryId,decimal? latitude,decimal? longitude,bool isHasPool,
            bool isHasElevator,bool isHaLoan,string userId,bool isPrivate,int regionId,string address,bool isShowLocation,
           DocumentTypeEnum documentType,  bool isRenovated=false
            )
        {
            Title = title;
            DescriptionRows= desc;
            RoomCount= roomCount;
            RegionId = regionId;
            IsShowLocation = isShowLocation;
            Latitude = latitude;
            Longitude=longitude;
            Price = price;
            Floor = floor;
            CountFloor=countFloor;
            CountInFloor=countInFloor;
            ConstructionYear = constructionYear;
            Rent = rent;
            CategoryId= categoryId;
            IsHasPool = isHasPool;
            IsHasElevator = isHasElevator;
            IsHaLoan = isHaLoan;
            UserId = Guid.Parse(userId);
            IsPrivate = isPrivate;
            IsShowLocation= isShowLocation;
            SquareMeter= squareMeter;
            Deposit = deposit;
            CreatedAt=DateTime.Now;
            Address= address;
            Status = RealEstateStatusEnum.WaitingForPayment;
            IsRenovated = isRenovated;
            DocumentType=documentType;

        }


    }
}
