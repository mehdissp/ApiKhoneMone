using AutoMapper;
using JWTApi.Application.DTOs.Categories;
using JWTApi.Application.DTOs.RealEstates;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Facilities;
using JWTApi.Domain.Dtos.ImageInfos;
using JWTApi.Domain.Dtos.RealEstate;
using JWTApi.Domain.Dtos.Regions;
using JWTApi.Domain.Dtos.Users;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Infrastructure.Repositories.Categories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.RealEstateses;
public class RealEstatesService
{
    private readonly IUnitOfWork _unit;
    private readonly IRealEstatesRepository _realEstatesRepository;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;
    public RealEstatesService(IRealEstatesRepository realEstatesRepository,
        IUnitOfWork unit, IMapper mapper,
        ICategoryRepository categoryRepository
        , IWalletRepository walletRepository, IUserRepository userRepository)
    {
        _realEstatesRepository = realEstatesRepository;
        _mapper = mapper;
        _unit = unit;
        _categoryRepository = categoryRepository;
        _walletRepository = walletRepository;
        _userRepository = userRepository;
    }
    public async Task<List<DTOs.RealEstates.RealEstateDto>> GetRandomLastItemRealEstates(int tabId, CancellationToken cancellation)
    {
        var result = await _realEstatesRepository.GetRandomLastItemRealEstates(tabId, cancellation);
        return _mapper.Map<List<DTOs.RealEstates.RealEstateDto>>(result);
    }
    public async Task<RealEstateDetails> GetRealEstateDetails(int id,string? userId, CancellationToken cancellationToken)
    {
        var result = await _realEstatesRepository.GetRealEstateDetails(id, userId, cancellationToken);
        return result;
    }
    public async Task<RealEstateDetailsEdit> GetRealEstateDetailsForEdit(int id, CancellationToken cancellationToken)
    {
        var result = await _realEstatesRepository.GetRealEstateDetailsForEdit(id, cancellationToken);
        return result;
    }
    public async Task<List<RealEstatePanel>> GetRealEstatePanel(string userId, CancellationToken cancellationToken)
    {
        var result = await _realEstatesRepository.GetRealEstatePanel(userId, cancellationToken);
        return result;
    }

    public async Task<List<FacilitiesDtos>> GetFacilitiesDtos(int catId, CancellationToken cancellationToken)
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

    public async Task<PagedResult<RealEstateMap>> GetRealStateMap(
  int tabId,
  int pageNumber = 1,
  int pageSize = 10,
  CancellationToken cancellationToken = default)
    {
        var result = await _realEstatesRepository
      .GetRealStateMap(
          tabId,
          pageNumber,
          pageSize,
          cancellationToken);

        var mappedItems = _mapper.Map<List<RealEstateMap>>(result.Items);

        return new PagedResult<RealEstateMap>
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
    public async Task<List<RegionDtos>> GetRegions(CancellationToken cancellationToken)
    {
        return await _realEstatesRepository.GetRegions(cancellationToken);
    }

    public async Task InsertRealEstate(RealEstateRequest realEstateRequest, string currnetUser, string roleId, List<ImagesInfo>? imageInfo, CancellationToken cancellationToken)
    {
        RealEstates realEstates = new RealEstates();
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


        int id = await _realEstatesRepository.InsertRealEstate(realEstates, facilityIds, imageInfo);
        var getAdPrice = await _realEstatesRepository.getAdPriceRange(Guid.Parse(roleId), realEstateRequest.CategoryTypeId);
        var resultWallet = await _walletRepository.WithdrawAsync(Guid.Parse(currnetUser), getAdPrice.AdPostingCost, "");
        if (resultWallet.IsSuccess)
        {
            //RealEstates real = await _realEstatesRepository.GetRealEstates(id, cancellationToken);
            //real.UpdateStatus(0);
            //await _unit.SaveChanges(cancellationToken);
            await UpdateStatusRealEstate(id, cancellationToken);
        }
    }

    public async Task<bool> CheckAccessToRealEstate(int id, string userId, string roleName, CancellationToken cancellationToken)
    {
        return await _realEstatesRepository.CheckAccessToRealEstate(id, userId, roleName, cancellationToken);
    }

    public async Task<PaymentStatusDtos> GetPaymentStatus(int realEstateId, string roleId, string userId)
    {
        return await _realEstatesRepository.GetPaymentStatus(realEstateId, Guid.Parse(roleId), Guid.Parse(userId));
    }

    public async Task UpdateStatusRealEstate(int id,CancellationToken cancellationToken)
    {
        RealEstates real = await _realEstatesRepository.GetRealEstates(id, cancellationToken);
        real.UpdateStatus(0);
        await _unit.SaveChanges(cancellationToken);
    }

    public async Task ToggleBookMark(string userId, int realEstateId, CancellationToken cancellationToken)
    {
        var bookMark = new BookMark();
        bookMark.Create(userId, realEstateId);

        var exists = await _realEstatesRepository.DeleteBookMark(bookMark, cancellationToken);

        if (!exists)
        {
            await _realEstatesRepository.InsertBookMark(bookMark, cancellationToken);
        
        }
        await _unit.SaveChanges(cancellationToken);
    }
    public async Task InsertViolations(string userId,int id,string? desc,int errorType, CancellationToken cancellationToken)
    {
            Violation violation = new Violation();
            violation.Create(userId, id, desc, errorType);
            await _realEstatesRepository.InsertViolations(violation, cancellationToken);
            await _unit.SaveChanges(cancellationToken);
    }


  public async  Task<UserForSite> GetUserForSite(string userId,CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserForSite(userId, cancellationToken);
    }

    public async Task<PagedResult<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithUser(
        string userId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _realEstatesRepository
      .GetRandomLastItemRealEstatesWithUser(
          userId,
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


   public async Task<PagedResult<RealEstatePanel>> GetRealEstateBookMark(
string userId,
int pageNumber = 1,
int pageSize = 10,
CancellationToken cancellationToken = default)
    {
        return await _realEstatesRepository.GetRealEstateBookMark(userId, pageNumber, pageSize, cancellationToken);
    }

    public async Task<List<IndependentAgentDtos>> GetIndependentAgent(CancellationToken cancellationToken)
    {
        return await _userRepository.GetIndependentAgent(cancellationToken);
    }

    public async Task<List<RegionParentDto>> GetRegionsWithChildrenLinq(int regionId)
    {
        return await _realEstatesRepository.GetRegionsWithChildrenLinq(regionId);
    }

    public async Task<PagedResult<RealEstateWithCategoryDto>> GetFilteredRealEstatesWithCategoryFilterAsync(
        FilterRealEstateDto filter, string? userId,
        CancellationToken cancellationToken = default)
    {
        // تبدیل FilterRealEstateDto به FilterRealEstateAllDto
        var filterAll = new FilterRealEstateAllDto
        {
            TabId = filter.TabId,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            ChildIds = filter.ChildIds,
            ConstructionYears = filter.ConstructionYears,
            PriceMin = filter.PriceMin,
            PriceMax = filter.PriceMax,
            AreaMin = filter.AreaMin,
            AreaMax = filter.AreaMax,
            YearMin = filter.YearMin,
            YearMax = filter.YearMax,
            FloorMin = filter.FloorMin,
            FloorMax = filter.FloorMax,
            RoomMin = filter.RoomMin,
            RoomMax = filter.RoomMax,
            IsHasElevator = filter.IsHasElevator,
            IsHasParking = filter.IsHasParking,
            IsHasPool = filter.IsHasPool,
            IsHasStoreRoom = filter.IsHasStoreRoom,
            SortBy = filter.SortBy,
            RegionId=filter.RegionId
        };

        return await _realEstatesRepository.GetFilteredRealEstatesWithCategoryFilterAsync(filterAll, userId);
    }


    public async Task UpdateViewCount(int id, CancellationToken cancellationToken)
    {
        await _realEstatesRepository.UpdateViewCount(id, cancellationToken);
    }


    public async Task<List<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithSimpleAsync
        (
     int regionId)
    {
        return await _realEstatesRepository.GetRandomLastItemRealEstatesWithSimpleAsync(regionId);
    }


  public  async Task<List<RealEstateWithCategoryDto>> GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync()
    {
        return await _realEstatesRepository.GetRandomLastItemRealEstatesWithTabIdVipSimpleAsync();
    }
}


