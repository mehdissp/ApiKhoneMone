using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstatesesApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.RealEstatesApplications
{
    public interface IRealEstatesApplicationsRepository
    {
        Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(
             string userId,
             int pageNumber = 1,
             int pageSize = 10,
             CancellationToken cancellationToken = default);
    }
}
