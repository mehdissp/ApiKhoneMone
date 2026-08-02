using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.RealEstateses
{
    public interface IRealEstatesRepository
    {
        Task<List<RealEstateDto>> GetRandomLastItemRealEstates(int tabId, CancellationToken cancellation);
        Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
            int tabId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<RealEstateDetails> GetRealEstateDetails(int id,string? userId, CancellationToken cancellationToken);
        Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken);

        Task<List<FacilitiesDtos>> GetFacilitiesDtos(int catId, CancellationToken cancellationToken);


        Task<List<RegionDtos>> GetRegionsWithChildFlagAsync(int? id,CancellationToken cancellationToken);

        Task<int> InsertRealEstate(RealEstates realEstates, List<int> facilityIds, List<ImagesInfo> images);

        Task<bool> CheckAccessToRealEstate(int id, string userId, string roleName, CancellationToken cancellationToken);
        Task<RealEstateDetailsEdit> GetRealEstateDetailsForEdit(int id, CancellationToken cancellationToken);
        Task<List<RegionDtos>> GetRegions(CancellationToken cancellationToken);
        Task<PagedResult<RealEstateMap>> GetRealStateMap(
int regionId,
int pageNumber = 1,
int pageSize = 10,
CancellationToken cancellationToken = default);
        Task<RealEstates> GetRealEstates(int id, CancellationToken cancellationToken);
        Task<AdPriceRanges> getAdPriceRange(Guid roleId,int categoryId);

        Task<PaymentStatusDtos> GetPaymentStatus(int realEstateId, Guid roleId, Guid userId);
        Task InsertBookMark(BookMark bookMark, CancellationToken cancellationToken);
        Task<bool> DeleteBookMark(BookMark bookMark, CancellationToken cancellationToken);
        Task InsertViolations(Violation violation, CancellationToken cancellationToken);
        Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithUser(
string userId,
int pageNumber = 1,
int pageSize = 10,
CancellationToken cancellationToken = default);


        Task<PagedResult<RealEstatePanel>> GetRealEstateBookMark(
    string userId,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);
        Task<List<RegionParentDto>> GetRegionsWithChildrenLinq(int regionId,CancellationToken cancellationToken = default);

        Task<PagedResult<RealEstateWithCategoryDto>> GetFilteredRealEstatesWithCategoryFilterAsync(
    FilterRealEstateAllDto filter, string? userId,
    CancellationToken cancellationToken = default);

        Task UpdateViewCount(int id, CancellationToken cancellationToken);

      Task<List<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithSimpleAsync(
           int regionId,
           CancellationToken cancellationToken = default);

        Task<List<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync(
            CancellationToken cancellationToken = default);
    }
}
