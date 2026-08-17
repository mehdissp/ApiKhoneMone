using JWTApi.Application.DTOs;
using JWTApi.Application.Helper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.ProjectUsers;
using JWTApi.Domain.Dtos.Users;
using JWTApi.Domain.Dtos.Wallets;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.TokenBlacklist;
using JWTApi.Domain.Interfaces.Wallets;
using JWTApi.Infrastructure.Repositories.Wallets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Application.Services;

public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unit;
    private readonly IPasswordHasher<User> _hasher;
    private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
    private readonly IWalletRepository _WalletRepository;


    public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
        ITokenBlacklistRepository tokenBlacklistRepository,
        IPasswordHasher<User> hasher, IUnitOfWork unit,
        IWalletRepository walletRepository)
    {
        _userRepo = userRepository;
        _hasher = hasher;
        _unit = unit;
        _tokenBlacklistRepository = tokenBlacklistRepository;
        _httpContextAccessor = httpContextAccessor;
        _WalletRepository = walletRepository;

    }
    public async Task<(bool Success, string Message)> RegisterAsync(RegisterNewUserDto dto, string userId, CancellationToken cancellationToken)
    {
        if (await _userRepo.GetByUsernameAsync(dto.Username, cancellationToken) != null)
            return (false, "User already exists");
        if (await _userRepo.checkMobileDublicated(dto.MobileNumber, cancellationToken) == true)
            return (false, "MobileNumber already exists");

        var user = new User(dto.Username, dto.Email, dto.IsActive, dto.MobileNumber, dto.fullname);
        user.SetPassword(_hasher.HashPassword(user, dto.Password));
        await _userRepo.AddUserWithAnotherUsers(user, userId, dto.RoleId, cancellationToken);
        //await _unit.SaveChanges(cancellationToken);
        return (true, "User created successfully");
    }

    public async Task<(bool Success, string Message)> RegisterIndependentAsync(RegisterIndepentAgent dto, CancellationToken cancellationToken)
    {
        if (await _userRepo.GetByUsernameAsync(dto.UserName, cancellationToken) != null)
            return (false, "User already exists");
        if (await _userRepo.checkMobileDublicated(dto.MobileNumber, cancellationToken) == true)
            return (false, "MobileNumber already exists");
        if (!NationalCodeValidator.IsValidNationalCode(dto.NationalCode))
            return (false, "کدملی اشتباه");

        User user = new User();
        user.createIndepent(dto.UserName, dto.FullName, dto.FullName, dto.NationalCode, dto.MobileNumber, true, 2, dto.CodeMoaref);
        user.SetPassword(_hasher.HashPassword(user, dto.PassWord));
        await _userRepo.AddUserIndepent(user, cancellationToken);
        await _WalletRepository.CreateWalletIndepentAsync(user.Id);
        //await _unit.SaveChanges(cancellationToken);
        return (true, "عملیات با موفقبت انجام شد");
    }

    public async Task<(bool Success, string Message)> RegisterRealEstateAgentAsync(RegisterRealEstateAgent dto, CancellationToken cancellationToken)
    {
        if (await _userRepo.GetByUsernameAsync(dto.UserName, cancellationToken) != null)
            return (false, "User already exists");
        if (await _userRepo.checkMobileDublicated(dto.MobileNumber, cancellationToken) == true)
            return (false, "MobileNumber already exists");
        if (!NationalCodeValidator.IsValidNationalCode(dto.NationalCode))
            return (false, "کدملی اشتباه");

        User user = new User();
        user.createIndepent(dto.UserName, dto.FullName, dto.FullName, dto.NationalCode, dto.MobileNumber, true, 1, dto.CodeMoaref);
        RealEstateAgentProfile profile = new RealEstateAgentProfile();
        
        user.SetPassword(_hasher.HashPassword(user, dto.PassWord));
        profile.CreateRealEstateAgentProfile(user.Id, dto.AgentCode, dto.NationalCartNumber, dto.OfficeAddress, dto.LicenseNumber, dto.LicenseExpiryDate, 5, 0, 0, 0);
        await _userRepo.AddUserRealEstateAgent(user, profile, cancellationToken);
   
        await _WalletRepository.CreateWalletIndepentAsync(user.Id);
        await _unit.SaveChanges(cancellationToken);
        return (true, "عملیات با موفقبت انجام شد");
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(UpdateNewUserDto dto, string userId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(dto.UserId, out Guid userGuidId))
            return (false, "Invalid user ID format");

        if (!Guid.TryParse(dto.RoleId, out Guid roleGuidId))
            return (false, "Invalid role ID format");
        if (await _userRepo.checkUserNameDublicatedUpdate(dto.Username, dto.UserId, cancellationToken))
            return (false, "User already exists");
        if (await _userRepo.checkMobileDublicatedUpdate(dto.MobileNumber, dto.UserId, cancellationToken) == true)
            return (false, "MobileNumber already exists");
        var user = await _userRepo.GetByUserIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            return (false, "User not found");
        user.UpdateUserInfo(dto.fullname, dto.Username, dto.MobileNumber, dto.IsActive);
        if (!dto.IsActive)
        {
            await _tokenBlacklistRepository.BlacklistAllUserTokensAsync(userId);
        }
        if (dto.IsChangePassword == true)
        {
            user.SetPassword(_hasher.HashPassword(user, dto.Password));
        }
        UserRole userRole = new UserRole()
        {
            UserId = userGuidId,
            RoleId = roleGuidId
        };

        await _userRepo.EditRole(userRole, cancellationToken);
        await _unit.SaveChanges(cancellationToken);
        return (true, "User created successfully");
    }

    public async Task<PagedResult<GetNewUserDto>> GetNewUser(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var result = await _userRepo.GetUsersAsync(userId, pageNumber, pageSize, cancellationToken);

        var users = result.Items.Select(user =>
        {
            var userRole = user.UserRoles.FirstOrDefault();
            return new GetNewUserDto(
                Id: user.Id,
                Username: user.Username,
                Email: user.Email,
                IsActive: user.IsActive,
                MobileNumber: user.MobileNumber,
                createdAt: user.CreatedAt,
                fullname: user.FullName,
                roleName: userRole?.Role?.Name ?? string.Empty,
                avatar: user.Avatar, // تغییر این خط
                roleId: userRole?.Role?.Id.ToString() ?? string.Empty
            );
        }).ToList();

        return new PagedResult<GetNewUserDto>
        {
            Items = users,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,

        };
    }


    public async Task<PagedResult<GetNewUserDto>> GetUsersForComboAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var result = await _userRepo.GetUsersForComboAsync(userId, pageNumber, pageSize, cancellationToken);

        var users = result.Items.Select(user =>
        {
            var userRole = user.UserRoles.FirstOrDefault();
            return new GetNewUserDto(
                Id: user.Id,
                Username: user.Username,
                Email: user.Email,
                IsActive: user.IsActive,
                MobileNumber: user.MobileNumber,
                createdAt: user.CreatedAt,
                fullname: user.FullName,
                roleName: userRole?.Role?.Name ?? string.Empty,
                avatar: user.Avatar, // تغییر این خط
                roleId: userRole?.Role?.Id.ToString() ?? string.Empty
            );
        }).ToList();


        return new PagedResult<GetNewUserDto>
        {
            Items = users,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,

        };
    }

    public async Task<List<Role>> GetRole(CancellationToken cancellationToken)
    {
        return await _userRepo.GetRoleCombo(cancellationToken);
    }

    public async Task<PagedResult<ProjectUserDtos>> GetProjectUserDtosAsync(string userId, int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _userRepo.GetProjectUserDtos(userId, projectId, pageNumber, pageSize, cancellationToken);
    }

    public async Task<string> UploadAvatarUser(string userId, IFormFile formFile, CancellationToken cancellation)
    {
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/users");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        // اگر کاربر فایل آواتار قبلی دارد، آن را حذف کن
        var user = await _userRepo.GetByUserIdAsync(userId, cancellation);
        if (!string.IsNullOrEmpty(user.Avatar))
        {
            // استخراج نام فایل از لینک کامل قبلی
            var oldFileName = Path.GetFileName(user.Avatar);
            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFilePath = Path.Combine(uploadsPath, oldFileName);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }
        }

        var fileName = $"{Guid.NewGuid()}_{formFile.FileName}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream);
        }

        // ساخت لینک کامل
        var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        var fullUrl = $"/uploads/users/{fileName}";

        user.UpdateUserProfile(fullUrl); // ذخیره لینک کامل
        await _unit.SaveChanges(cancellation);

        return fullUrl; // بازگشت لینک کامل
    }


    public async Task<UserDashboard> getUserDashbaordIndependent(string userId)
    {
        return await _userRepo.getUserDashbaordIndependent(userId);
    }

    public async Task<List<TransactionDtos>> GetTransactionHistoryAsync(Guid userId,int pageSize,int pageNumber,CancellationToken cancellationToken)
    {
        return await _WalletRepository.GetTransactionHistoryAsync(userId, cancellationToken, pageNumber,pageSize);
    }
    public async Task<WalletResult> GetBalanceAsync(Guid userId)
    {
        return await _WalletRepository.GetBalanceAsync(userId);
    }

    
    public async Task<string> getAvatar(string userId, CancellationToken cancellationToken)
    {
        return await _userRepo.getAvatar(userId, cancellationToken);
    }




}

