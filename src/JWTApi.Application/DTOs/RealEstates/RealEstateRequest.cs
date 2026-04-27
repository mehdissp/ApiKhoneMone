using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.RealEstates
{
    public class RealEstateRequest
    {
        public string Title { get; set; }
        public int Region { get; set; }
        public int Sqmeter { get; set; }
        public int CountFloor { get; set; }
        public int CountInFloor { get; set; }
        public int Floor { get; set; }
        public int CountRoom { get; set; }
        public bool IsHasElevator { get; set; } = false;
        public bool IsHasStoreRoom { get; set; } = false;
        public bool IsHasParking { get; set; } = false;
        public bool IsHaLoan { get; set; } = false;
        public decimal? lat { get; set; }
        public decimal? lon { get; set; }
        public string Address { get; set; }
        public string DescriptionRows { get; set; }
        public List<RealEstate_Facilities> Facilities { get; set; }
        public bool ShowExactLocation { get; set; }
        public int CategoryTypeId { get; set; }
        public long Price { get; set; }
        public long RentPrice { get; set; }
        public long DepositPrice { get; set; }
        public bool IsMortgageOnly { get; set; }
        public int ContractDuration { get; set; }
        public List<string> TempImageCacheIds { get; set; } = new List<string>();

    }
    public class RealEstate_Facilities()
    {
        public int Id { get; set; }
    }
}
