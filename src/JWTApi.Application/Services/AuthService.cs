using JWTApi.Application.DTOs;
using JWTApi.Application.Helper;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JWTApi.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _hasher;
        private readonly JwtService _jwtService;

        public AuthService(IUserRepository userRepo, IPasswordHasher<User> hasher, JwtService jwtService)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            if (await _userRepo.GetByUsernameAsync(dto.Username,cancellationToken) != null)
                return (false, "User already exists");

            var user = new User(dto.Username, dto.Email);
            user.SetPassword(_hasher.HashPassword(user, dto.Password));
            await _userRepo.AddAsync(user, cancellationToken);
            return (true, "User created successfully");
        }

        //public async Task<(bool Success, string? Token, string? RefreshToken,DateTime? ExpiresAt)> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
        //{
        //    var user = await _userRepo.GetByUsernameAsync(dto.Username, cancellationToken);
        //    if (user == null) return (false, null, null,null);

        //    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        //    if (result == PasswordVerificationResult.Failed)
        //        return (false, null, null, null);

        //    var (token, expiresAt) = _jwtService.GenerateToken(user);
        //    user.RefreshToken = Guid.NewGuid().ToString();
        //    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        //    await _userRepo.UpdateAsync(user);

        //    return (true, token, user.RefreshToken, expiresAt);
        //}
        public async Task<(bool Success, string? Token, string? RefreshToken, DateTime? Expiry)> LoginAsync(LoginDto dto,string ip, CancellationToken cancellationToken)
        {

            var user = await _userRepo.GetByUsernameAsync(dto.Username,cancellationToken);
           // if (user == null) return (false, null, null, null);
            if (user == null)
            {
               // await LogLoginAttempt(dto.Username, ip, false, "User not found",cancellationToken);
           
                return (false, null, null, null);
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                //await LogLoginAttempt(dto.Username, ip, false, "Wrong password", cancellationToken);
                return (false, null, null, null);
            }

            //await LogLoginAttempt(dto.Username, ip, true, "Login successful", cancellationToken );
            var roles = await _userRepo.GetUserRolesAsync(user.Id, cancellationToken);
            var (token, expiry) = _jwtService.GenerateToken(user, roles);

            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepo.UpdateAsync(user);

            return (true, token, user.RefreshToken, expiry);
        }

        private async Task LogLoginAttempt(string? username, string? ip, bool isSuccess, string? reason,CancellationToken cancellationToken)
        {
            var attempt = new LoginAttempt
            {
                Username = username,
                IPAddress = ip,
                AttemptTime = DateTime.Now,
                Success = isSuccess,
                Reason = reason
            };
            await _userRepo.AddLoginAttemptAsync(attempt, cancellationToken);
            await _userRepo.CheckAndLockIp(ip);
        }

        public async Task<(bool Success, string? Token, string? RefreshToken, DateTime? ExpiresAt)> RefreshAsync(RefreshDto dto, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username, cancellationToken);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return (false, null, null,null);
            var roles = await _userRepo.GetUserRolesAsync(user.Id,cancellationToken);
            var (token, expiresAt) = _jwtService.GenerateToken(user, roles);
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepo.UpdateAsync(user);

            return (true, token, user.RefreshToken, expiresAt);
        }

        public async Task<UserProfileDto> GetUserProfile(string userId,CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByUserIdAsync(userId, cancellationToken);
            if (user == null) return null;

            // فقط فیلدهای امن برمی‌گردد
            return new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email,
              //  ProfileImagePath = user.ProfileImagePath
              MobileNumber=user.MobileNumber,
              FullName=user.FullName,
              CreatedAt=user.CreatedAt,
              Avatar=user.Avatar
            };
        }

        public async Task<List<MenuPermissionDto>> GetUserMenuPermissionsAsync(string userId ,CancellationToken cancellationToken)
        {
            return await _userRepo.GetUserMenuPermissionsAsync(userId, cancellationToken);
        }

        public async Task<List<MenuUi>> GetUserMenuPermissionsForUiAsync(string userId,string roleId, CancellationToken cancellationToken)
        {
            return await _userRepo.GetUserMenuPermissionsForUiAsync(userId, roleId, cancellationToken);
        }
        
    }



}
