using AutoMapper;
using JWTApi.Application.DTOs.Categories;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Infrastructure.Repositories.Categories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.RealEstateses
{
    public class RealEstatesService
    {
        private readonly IUnitOfWork _unit;
        private readonly IRealEstatesRepository _realEstatesRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        public RealEstatesService(IRealEstatesRepository realEstatesRepository, IUnitOfWork unit, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _realEstatesRepository = realEstatesRepository;
            _mapper = mapper;
            _unit = unit;
            _categoryRepository = categoryRepository;
        }
        public async Task<List<DTOs.RealEstates.RealEstateDto>> GetRandomLastItemRealEstates(int tabId, CancellationToken cancellation)
        {
                var result = await _realEstatesRepository.GetRandomLastItemRealEstates(tabId,cancellation);
                return _mapper.Map<List<DTOs.RealEstates.RealEstateDto>>(result);
        }
        public async Task<RealEstateDetails> GetRealEstateDetails(int id, CancellationToken cancellationToken)
        {
            var result = await _realEstatesRepository.GetRealEstateDetails(id, cancellationToken);
            return result;
        }
        public async Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken)
        {
            var result = await _realEstatesRepository.GetRealEstatePanel(userId, cancellationToken);
            return result;
        }

        public  async Task<List<FacilitiesDtos>> GetFacilitiesDtos(int catId, CancellationToken cancellationToken)
        {
            return await _realEstatesRepository.GetFacilitiesDtos(catId, cancellationToken);
        }


        public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithCategoryAsync(
            int tabId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _realEstatesRepository
          .GetRandomLastItemRealEstatesWithCategoryAsync(
              tabId,
              pageNumber,
              pageSize,
              cancellationToken);

            var mappedItems = _mapper.Map<List<RealEstateWithCategoryDto>>(result.Items);

            return new PagedResult<RealEstateWithCategoryDto>
            {
                Items = mappedItems,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            };
        }
        public async Task<List<CategoryDto>> GetCategoryDtos(int tabId, CancellationToken cancellation)
        {
                var result = await _categoryRepository.GetCategoryDtos(tabId, cancellation);
                return _mapper.Map<List<CategoryDto>>(result);
         
        }

        public async Task<List<RegionDtos>> GetRegionsWithChildFlagAsync(int? id, CancellationToken cancellationToken)
        {
            return await _realEstatesRepository.GetRegionsWithChildFlagAsync(id, cancellationToken);
        }

        public async Task InsertRealEstate(RealEstateRequest realEstateRequest,string currnetUser,List<ImagesInfo>? imageInfo ,CancellationToken cancellationToken)
        {
           RealEstates realEstates=new RealEstates();
            realEstates.create(realEstateRequest.Title, realEstateRequest.DescriptionRows, realEstateRequest.CountRoom,
                realEstateRequest.Floor, realEstateRequest.CountFloor, realEstateRequest.CountInFloor, realEstateRequest.ContractDuration
                , realEstateRequest.Sqmeter, realEstateRequest.Price, realEstateRequest.DepositPrice, realEstateRequest.RentPrice
                , realEstateRequest.CategoryTypeId, realEstateRequest.lat, realEstateRequest.lon, false, realEstateRequest.IsHasElevator
                , realEstateRequest.IsHaLoan, currnetUser, false, realEstateRequest.Region, realEstateRequest.Address,
                realEstateRequest.ShowExactLocation, realEstateRequest.DocumentType, realEstateRequest.IsRenovated);
            // استخراج فقط شناسه‌های امکانات
            List<int> facilityIds = realEstateRequest.Facilities
                .Select(f => f.Id) // یا هر property که ID در آن است
                .ToList();


            await _realEstatesRepository.InsertRealEstate(realEstates, facilityIds, imageInfo);
        }


    }

}
