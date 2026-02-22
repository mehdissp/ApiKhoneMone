using AutoMapper;
using JWTApi.Application.DTOs.Categories;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.RealEstates;
using JWTApi.Infrastructure.Repositories.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.RealEstates
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
    }

}
