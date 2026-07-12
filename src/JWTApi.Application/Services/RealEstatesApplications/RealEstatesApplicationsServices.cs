using AutoMapper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstatesesApplications;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.RealEstatesApplications;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Domain.Interfaces.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.RealEstatesApplications
{
    public class RealEstatesApplicationsServices
    {
        private readonly IUnitOfWork _unit;
        private readonly IRealEstatesRepository _realEstatesRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRealEstatesApplicationsRepository _realEstatesApplicationsRepository;
        public RealEstatesApplicationsServices(IRealEstatesRepository realEstatesRepository,
            IUnitOfWork unit, IMapper mapper,
            ICategoryRepository categoryRepository
            , IWalletRepository walletRepository, IUserRepository userRepository, IRealEstatesApplicationsRepository realEstatesApplicationsRepository)
        {
            _realEstatesRepository = realEstatesRepository;
            _mapper = mapper;
            _unit = unit;
            _categoryRepository = categoryRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _realEstatesApplicationsRepository= realEstatesApplicationsRepository;
        }

        public async Task<PagedResult<RealEstatesesApplicationsDto>> RealEstatesesApplicationsDtosAsync(string userId, string searchTrem,int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _realEstatesApplicationsRepository.RealEstatesesApplicationsDtosAsync(userId, searchTrem, pageNumber,pageSize, cancellationToken);
        }
        public async Task<RealEstatesesApplicationsDetailsDto> GetRealEstatesesApplicationsDetails(int id, string userId, CancellationToken cancellationToken)
        {
            return await _realEstatesApplicationsRepository.GetRealEstatesesApplicationsDetails(id, userId, cancellationToken);
        }

        public async Task<PaymentStatusDtos> GetPaymentStatus(int realEstateIdAppId, string roleId, string userId, CancellationToken cancellationToken)
        {
            return await _realEstatesApplicationsRepository.GetPaymentStatus(realEstateIdAppId, Guid.Parse(roleId), Guid.Parse(userId), cancellationToken);
        }
    }
}
