using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.RealEstates
{
    public interface IRealEstatesRepository
    {
        Task<List<RealEstateDto>> GetRandomLastItemRealEstates(int tabId, CancellationToken cancellation);
        Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
            int tabId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

    }
}
