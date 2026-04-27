using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
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

        Task<RealEstateDetails> GetRealEstateDetails(int id, CancellationToken cancellationToken);
        Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken);

        Task<List<FacilitiesDtos>> GetFacilitiesDtos(int catId, CancellationToken cancellationToken);


        Task<List<RegionDtos>> GetRegionsWithChildFlagAsync(int? id,CancellationToken cancellationToken);

        Task InsertRealEstate(RealEstates realEstates, List<int> facilityIds, List<ImagesInfo> images);

    }
}
