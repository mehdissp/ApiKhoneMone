using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using JWTApi.Domain.Dtos;
using JWTApi.Infrastructure.Exceptions;
using JWTApi.Domain.Dtos.ProjectUsers;
using System.Threading;

namespace JWTApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
          return  await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true, cancellationToken);
        }
         
        public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    =>  await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role) // برای دسترسی به نام نقش
            .Select(ur => ur.Role.Name)
            .ToListAsync();
        public async Task<User?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
       => await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId, cancellationToken);

        public async Task<User?> GetByUserIdAsyncForToken(string userId)
=> await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
          await  _context.Users.AddAsync(user,cancellationToken );
            await _context.SaveChangesAsync();
        }

        //public async Task AddUserWithAnotherUsers(User user,string userId,string roleId,CancellationToken cancellationToken)
        //{
        //    var currentProjects = await _context.Users.CountAsync(p => p.UserId.ToString() == userId, cancellationToken);
        //    // ۲. مجموع پروژه‌های مجاز از پکیج‌ها
        //    var totalFromPackages = await _context.UserPackages
        //        .Where(up => up.UserId.ToString() == userId)
        //        .Include(up => up.Package)
        //        .SumAsync(up => (int?)up.Package.MaxUsers) ?? 0;

        //    // ۳. مجموع پروژه‌های خرید اضافه
        //    var totalExtra = await _context.ExtraProjects
        //        .Where(ep => ep.UserId.ToString() == userId)
        //        .SumAsync(ep => (int?)ep.CountUsers) ?? 0;
        //    var totalAllowed = totalFromPackages + totalExtra;

        //    // ۴. بررسی محدودیت
        //    if (currentProjects >= totalAllowed)
        //    {
        //        throw new RestBasedException("شما به حداکثر تعداد کاربر مجاز خود رسیده‌اید.", 402);
        //    }
        //    user.UserId = Guid.Parse(userId);
        //    await _context.AddAsync(user, cancellationToken);
        //    await _context.SaveChangesAsync(cancellationToken);
        //    UserRole userRole = new UserRole
        //    {
        //        UserId = user.Id,
        //        RoleId = Guid.Parse(roleId)
        //    };
        //    await _context.UserRoles.AddAsync(userRole, cancellationToken);
        //    await _context.SaveChangesAsync(cancellationToken);
        //}

        public async Task AddUserWithAnotherUsers(User user, string userId, string roleId, CancellationToken cancellationToken)
        {
            // 1. تبدیل شناسه‌ها به Guid
            var userGuid = Guid.Parse(userId);
            var roleGuid = Guid.Parse(roleId);

            // 2. بررسی محدودیت تعداد کاربران
            //var currentUsersCount = await _context.Users
            //    .CountAsync(u => u.UserId == userGuid, cancellationToken);

            //var totalFromPackages = await _context.UserPackages
            //    .Where(up => up.UserId == userGuid)
            //    .Include(up => up.Package)
            //    .SumAsync(up => (int?)up.Package.MaxUsers, cancellationToken) ?? 0;

            //var totalExtra = await _context.ExtraProjects
            //    .Where(ep => ep.UserId == userGuid)
            //    .SumAsync(ep => (int?)ep.CountUsers, cancellationToken) ?? 0;

            //var totalAllowed = totalFromPackages + totalExtra;

            //if (currentUsersCount >= totalAllowed)
            //{
            //    throw new RestBasedException("شما به حداکثر تعداد کاربر مجاز خود رسیده‌اید.", 402);
            //}

            // 3. استفاده از تراکنش برای عملیات اتمیک
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 4. افزودن کاربر جدید
                user.UserId = userGuid;
                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 5. اختصاص نقش به کاربر
                var userRole = new UserRole
                {
                    UserId = user.Id, // فرض می‌کنیم User.Id خودکار تولید می‌شود
                    RoleId = roleGuid
                };

                await _context.UserRoles.AddAsync(userRole, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 6. تأیید تراکنش
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task EditRole(UserRole userRole,CancellationToken cancellationToken)
        {
            var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(s =>  s.UserId == userRole.UserId,cancellationToken);
            if (existingUserRole != null)
            {
                // فقط اگر نقش تغییر کرده باشد به‌روزرسانی کن
                if (existingUserRole.RoleId != userRole.RoleId)
                {
                    //existingUserRole.RoleId = userRole.RoleId;
                    //_context.UserRoles.Update(existingUserRole);
                    _context.UserRoles.Remove(existingUserRole);
                    await _context.UserRoles.AddAsync(userRole, cancellationToken);

                }
            }
            else
            {
                await _context.UserRoles.AddAsync(userRole, cancellationToken);
            }
        }


     //public async Task UpdateUser(string userId,string fullName,string password,string userName,string mobileNumber,bool isActive,bool isChangePasssword,CancellationToken cancellationToken)
     //   {
     //       var user = await GetByUserIdAsync(userId, cancellationToken);
     //       user.FullName = fullName;
     //       user.Username = userName;
     //       if (isChangePasssword==false)
     //       {
     //           user.PasswordHash = password ?? string.Empty;
     //       }
     //       user.IsActive = isActive;
     //       user.MobileNumber = mobileNumber;
     //   }


        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuPermissionDto>> GetUserMenuPermissionsAsync(string userId, CancellationToken cancellationToken)
        {
            // نقش‌های کاربر
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId.ToString() == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            // گرفتن منوها و دسترسی‌ها
            var menuPermissions = await _context.Menus
                .Select(menu => new MenuPermissionDto
                {
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    Url = menu.Url,
    //                Permissions = _context.RoleMenus
    //.Where(rmp => userRoleIds.Contains(rmp.RoleId) && rmp.MenuId == menu.Id )
    //.Select(rmp => rmp.Permission.Name)
    //.Distinct()
    //.ToList()

                })
                .ToListAsync(cancellationToken);

            return menuPermissions;
        }


        public async Task<List<MenuUi>> GetUserMenuPermissionsForUiAsync(string userId,string roleId, CancellationToken cancellationToken)
         {
          

          
            var roleGuid = Guid.Parse(roleId);

            var menuPermissions = await _context.Roles
                .Where(r => r.Id == roleGuid)
                .SelectMany(r => r.RoleMenus.Select(rm => rm.Menu).Where(s=>s.IsMenu==true))
                .Where(m => m.ParentId == null).Select(menu => new MenuUi
                {
                    Id = menu.Id,
                    Path = menu.Path,
                    Label = menu.Label,
                    Icon = menu.Icon,

                })
                .ToListAsync();

 

            return menuPermissions;
        }
        public async Task AddLoginAttemptAsync(LoginAttempt loginAttempt, CancellationToken cancellationToken)
        {
            await _context.LoginAttempts.AddAsync(loginAttempt, cancellationToken);
            await _context.SaveChangesAsync();
        }
        public async Task CheckAndLockIp(string? ip)
        {
            if (string.IsNullOrEmpty(ip))
                return;

            var window = DateTime.Now.AddMinutes(-5); 
            var failCount = await _context.LoginAttempts
                .CountAsync(x => x.IPAddress == ip && !x.Success && x.AttemptTime > window);

            if (failCount >= 5)
            {
            
                var alreadyLocked = await _context.IpLocks.AnyAsync(x => x.IPAddress == ip && x.LockEnd > DateTime.Now);
                if (!alreadyLocked)
                {
                    var lockEntry = new IpLock
                    {
                        IPAddress = ip,
                        LockEndAt = DateTime.Now,
                        LockEnd = DateTime.Now.AddMinutes(5), 
                        Reason = $"Too many failed login attempts ({failCount})"
                    };
                    _context.IpLocks.Add(lockEntry);
                    await _context.SaveChangesAsync();
                }
            }
        }



        //public async Task<List<User>> GetUsersAsync(string userId, int PageNumber, int PageSize, CancellationToken cancellationToken)
        //{
        //    return await _context.Users.Where(s => s.UserId.ToString() == userId).ToListAsync(cancellationToken);
        //}
        //    public async Task<PagedResult<User>> GetUsersAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        //    {
        //        var query = _context.UserRoles.Include(s=>s.User).Include(s=>s.Role).Include(S=>S.User.UserPackages).ThenInclude(s=>s.Package).Where(s => s.UserId.ToString() == userId);

        //        var totalCount = await query.CountAsync(cancellationToken);

        //        var users = await query
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync(cancellationToken);



        //        var maxUsers = await _context.Users
        //.Where(u => u.Id.ToString() ==userId)
        //.SelectMany(u => u.UserPackages)
        //.Select(up => up.Package.MaxUsers)
        //.FirstOrDefaultAsync(cancellationToken);


        //        return new PagedResult<User>
        //        {
        //            Items = users,
        //            TotalCount = totalCount,
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,
        //            Max= maxUsers
        //        };
        //    }

        
        public async Task<PagedResult<User>> GetUsersAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            // تبدیل userId به Guid
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            //// پیدا کردن کاربر اصلی و پکیج مربوطه
            //var currentUserPackage = await _context.Users
            //    .Where(u => u.Id == userGuid)
            //    .SelectMany(u => u.UserPackages)
            //    .Select(up => new { up.Package.MaxUsers })
            //    .FirstOrDefaultAsync(cancellationToken);

            //if (currentUserPackage == null)
            //{
            //    throw new Exception("User package not found");
            //}

            // کوئری برای گرفتن کاربران تحت مدیریت
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserPackages)
                    .ThenInclude(up => up.Package)
                .Where(u => u.UserId == userGuid); // یا منطق کسب و کار شما

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                //Max = currentUserPackage.MaxUsers
            };
        }

        public async Task<PagedResult<User>> GetUsersForComboAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            // تبدیل userId به Guid
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid user ID format");
            }

         

            // کوئری برای گرفتن کاربران تحت مدیریت
            var query = _context.Users
           
                .Where(u => u.UserId == userGuid && u.IsActive==true) ; 

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
             
            };
        }

        public async Task<bool> checkUserNameDublicated(string userName,CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.Username == userName);
        }
        public async Task<bool> checkMobileDublicated(string mobileNumber, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.MobileNumber == mobileNumber);
        }
        public async Task<bool> checkUserNameDublicatedUpdate(string userName,string userId, CancellationToken cancellationToken)
        {
            var t= await _context.Users.AnyAsync(s => s.Username == userName && s.Id.ToString()!=userId);
            return t;
        }
        public async Task<bool> checkMobileDublicatedUpdate(string mobileNumber, string userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(s => s.MobileNumber == mobileNumber && s.Id.ToString() != userId);
        }

        public async Task<List<Role>> GetRoleCombo(CancellationToken cancellationToken)
        {
            return await _context.Roles.Where(s => s.IsSeen == true).ToListAsync(cancellationToken);
        }


        public async Task<PagedResult<ProjectUserDtos>> GetProjectUserDtos(string userId,int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = from u in _context.Users
                            join r in _context.UserRoles on u.Id equals r.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty()
                            join q in _context.Roles on ur.RoleId equals q.Id into roles
                            from role in roles.DefaultIfEmpty()
                            where u.UserId.ToString() == GetUserIdManager(userId)
                            && u.Id.ToString() != userId
                            select new ProjectUserDtos
                            {
                                Id = u.Id,
                                FullName = u.FullName,
                                RoleName = role.Name,
                                IsCheck = _context.ProjectUsers.Any(pu => pu.UserId == u.Id && pu.ProjectId == projectId)
                            };

            // گرفتن تعداد کل رکوردها
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی
            var users = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProjectUserDtos>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private string GetUserIdManager(string userId)
        {

            var baseQuery = from u in _context.Users
                            join ur in _context.UserRoles on u.UserId equals ur.UserId into userRoles
                            from ur in userRoles.DefaultIfEmpty()
                            join r in _context.Roles on ur.RoleId equals r.Id into roles
                            from role in roles.DefaultIfEmpty()
                            where u.Id == Guid.Parse(userId) // فقط کاربرانی که نقش دارند
                            select new
                            {
                                User = u,
                                RoleId = ur.RoleId,
                                RoleName = role.Name,
                                Role = role
                            };
            var baseQuery2 = (from u in _context.Users
                              join ur in _context.UserRoles on u.Id equals ur.UserId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where u.Id == baseQuery.FirstOrDefault().User.UserId
                              select new
                              {
                                  User = u,
                                  RoleId = ur.RoleId,
                                  RoleName = r.Name,
                                  Role = r
                              }).ToList();
            if (baseQuery2.Count()==0)
            {
                return userId;
            }
            if (baseQuery2.FirstOrDefault().Role.TypeRole == 1)
            {
                return userId;
            }
            else
            {
                var t = baseQuery2.First().User.Id.ToString();
                return t;
            }

        }

    }
}
