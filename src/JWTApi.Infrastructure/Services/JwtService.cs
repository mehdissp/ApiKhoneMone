using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        public JwtService(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        //public string GenerateToken(User user)
        //{
        //    var jwt = _config.GetSection("Jwt");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //             new Claim("id", user.Id.ToString()),
        //          new Claim(ClaimTypes.Name, user.Username)
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: jwt["Issuer"],
        //        audience: jwt["Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddSeconds(30),
        //        signingCredentials: creds
                
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
        public (string Token, DateTime ExpiresAt) GenerateToken(User user, List<string> roles)
{
    var jwt = _config.GetSection("Jwt");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("id", user.Id.ToString()),
        new Claim("token_id", Guid.NewGuid().ToString()), // شناسه یکتا برای توکن
                  new Claim("token_version", user.TokenVersion.ToString()), // نسخه توکن
                new Claim("is_active", user.IsActive.ToString()) ,// وضعیت کاربر



        new Claim(ClaimTypes.Name, user.Username)
    };
            var firstRole = user.UserRoles.FirstOrDefault();
            if (firstRole != null)
            {
               /* claims.Add(new Claim(ClaimTypes.Role, firstRole.Role.Name))*/;
                claims.Add(new Claim("roleId", firstRole.RoleId.ToString()));
                claims.Add(new Claim("roleName", firstRole.Role.Name));
            }
            //// اضافه کردن نقش‌ها و دسترسی‌ها
            //foreach (var role in user.UserRoles.Select(ur => ur.Role))
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.Name));

            //    //foreach (var perm in role.RolePermissions.Select(rp => rp.Permission))
            //    //{
            //    //    claims.Add(new Claim("permission", perm.Code));
            //    //}
            //}
            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}



            // زمان انقضا
            var expires = DateTime.Now.AddHours(30);

    var token = new JwtSecurityToken(
        issuer: jwt["Issuer"],
        audience: jwt["Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // برگرداندن هم توکن و هم زمان انقضا
    return (tokenString, expires);
}

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var (isValid, _) = await ValidateAndExtractUserIdAsync(token);
                return isValid;
            }
            catch
            {
                return false;
            }
        }


        public async Task<(bool isValid, string userId)> ValidateAndExtractUserIdAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // استخراج اطلاعات از توکن
               // var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var tokenVersion = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_version")?.Value;
                var isActive = jwtToken.Claims.FirstOrDefault(c => c.Type == "is_active")?.Value;
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenVersion))
                    return (false, null);

                // بررسی کاربر در دیتابیس
                var user = await _userRepository.GetByUserIdAsyncForToken(userId);
                if (user == null)
                    return (false, null);

                // بررسی نسخه توکن
                if (user.TokenVersion != int.Parse(tokenVersion))
                    return (false, userId);

                // بررسی فعال بودن کاربر
                if (!user.IsActive)
                    return (false, userId);

                return (true, userId);
            }
            catch
            {
                return (false, null);
            }
        }


    }
}
