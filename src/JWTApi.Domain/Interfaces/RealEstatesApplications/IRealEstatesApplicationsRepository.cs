using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstatesesApplications;
using JWTApi.Domain.Dtos.Wallets;
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
                 string searchTerm = null,
             int pageNumber = 1,
             int pageSize = 10,
             CancellationToken cancellationToken = default);
        Task<RealEstatesesApplicationsDetailsDto> GetRealEstatesesApplicationsDetails(int id, string userId, CancellationToken cancellationToken);

        Task<PaymentStatusDtos> GetPaymentStatus(int realEstateIdAppId, Guid roleId, Guid userId, CancellationToken cancellationToken);

        Task AccessToShowMobileNumber(int realEstateIdAppId, Guid roleId, Guid userId, CancellationToken cancellationToken);
    }
}
