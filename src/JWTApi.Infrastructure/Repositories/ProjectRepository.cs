using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(string name,string userId, CancellationToken cancellationToken)
        {
            var currentProjects = await _context.Projects.CountAsync(p => p.UserId.ToString() == GetUserIdManager(userId), cancellationToken);

            // ۲. مجموع پروژه‌های مجاز از پکیج‌ها
            var totalFromPackages = await _context.UserPackages
                .Where(up => up.UserId.ToString() == GetUserIdManager(userId))
                .Include(up => up.Package)
                .SumAsync(up => (int?)up.Package.MaxProjects) ?? 0;

            // ۳. مجموع پروژه‌های خرید اضافه
            var totalExtra = await _context.ExtraProjects
                .Where(ep => ep.UserId.ToString() == GetUserIdManager(userId))
                .SumAsync(ep => (int?)ep.CountProject) ?? 0;
            var totalAllowed = totalFromPackages + totalExtra;
            // ۴. بررسی محدودیت
            if (currentProjects >= totalAllowed)
            {
                throw new RestBasedException("شما به حداکثر تعداد پروژه مجاز خود رسیده‌اید.", 403);
            }

            Project project = new Project();
            project.Name = name;
            project.UserId = Guid.Parse(userId);
            await _context.AddAsync(project,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            ProjectUser projectUser = new ProjectUser();
            projectUser.ProjectId = project.Id;
            projectUser.UserId = Guid.Parse(userId);
            projectUser.IsCreator = true;
            await _context.ProjectUsers.AddAsync(projectUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            // گرفتن پروژه به همراه بررسی وجود تو دوها

            var project = await _context.Projects
                
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project == null)
                throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);

       

            // حذف پروژه
            //_context.Projects.Remove(project);
            project.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<Project?> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.Projects.FindAsync(projectId, cancellationToken);
        }

        public async Task<(List<ProjectWithPackageInfoDto> Items,
            int TotalCount, int TotalPages, bool CheckAccess, 
            bool CheckAccessDelte, bool CheckAccessAssigner)> GetProjectsWithPackageInfo(
            string userId,
            string roleId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {


            var skip = (pageNumber - 1) * pageSize;

            var baseQuery = _context.ProjectUsers
                .Where(s => (s.UserId.ToString() == GetUserIdManager(userId) ) );
              //  .Include(s => s.User)
                //    .ThenInclude(u => u.UserPackages)
                 //   .ThenInclude(up => up.Package);

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var baseQueryOrdered = baseQuery
                                           .Skip(skip)
                                           .Take(pageSize);

            var hasAccess =await HasMenuAccessAsync(roleId, "/api/Project/InsertProject", cancellationToken);
            var hasAccessDelete = await HasMenuAccessAsync(roleId, "/api/Project/DeleteProject", cancellationToken);
            var hasAccessAssigner = await HasMenuAccessAsync(roleId, "/api/User/GetProjectUsers", cancellationToken);
            var maxProjects = hasAccess ?
               _context.UserPackages
    .Include(up => up.Package)
    .FirstOrDefault(up => up.UserId.ToString() == GetUserIdManager(userId))
    ?.Package?.MaxProjects ?? 0 : 0;

            var query = baseQueryOrdered.Select(s => new
            {
                Project = s,
                MaxProjects = maxProjects,
                CheckAccess = hasAccess,
                CheckAccessDelete= hasAccessDelete,
                CheckAccessAssigner= hasAccessAssigner
            });


            var result = await query.ToListAsync(cancellationToken);
            

            var items = result.Select((item, index) => new ProjectWithPackageInfoDto
            {
          
                MaxProjects = item.MaxProjects,
                CheckAccess=item.CheckAccess,
                CheckAccessDelete=item.CheckAccessDelete,
                CheckAccessAssigner=item.CheckAccessAssigner,
                RowNum = skip + index + 1
            }).ToList();

            return (items, totalCount, totalPages, hasAccess,hasAccessDelete,hasAccessAssigner);
        }


        //public async Task InsertOrDeleteUserInProject(List<ProjectUser> projectUsers, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // گرفتن اولین RoleId برای فیلتر کردن (فرض می‌کنیم همه آیتم‌ها RoleId یکسان دارند)
        //        var firstRoleId = projectUsers.First().ProjectId;

        //        // موجودی فعلی از دیتابیس
        //        var existingRoleMenus = await _context.ProjectUsers
        //            .Where(rm => rm.ProjectId == firstRoleId)
        //            .ToListAsync(cancellationToken);

        //        // پیدا کردن مواردی برای حذف (در دیتابیس هستند ولی در لیست جدید نیستند)
        //        var toDelete = existingRoleMenus
        //            .Where(erm => !projectUsers.Any(nrm =>
        //                nrm.ProjectId == erm.ProjectId && nrm.UserId == erm.UserId && erm.IsCreator ==false))
        //            .ToList();

        //        // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
        //        var toAdd = projectUsers
        //            .Where(nrm => !existingRoleMenus.Any(erm =>
        //                erm.ProjectId == nrm.ProjectId && erm.UserId == nrm.UserId))
        //            .ToList();

        //        // اجرای عملیات
        //        if (toDelete.Any())
        //        {

        //            _context.ProjectUsers.RemoveRange(toDelete);
        //        }

        //        if (toAdd.Any())
        //        {
        //            await _context.ProjectUsers.AddRangeAsync(toAdd, cancellationToken);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

        public async Task InsertOrDeleteUserInProject(List<ProjectUser> projectUsers, CancellationToken cancellationToken)
        {
            try
            {
                // گرفتن اولین ProjectId برای فیلتر کردن (فرض می‌کنیم همه آیتم‌ها ProjectId یکسان دارند)
                var firstProjectId = projectUsers.First().ProjectId;

                // موجودی فعلی از دیتابیس
                var existingProjectUsers = await _context.ProjectUsers
                    .Where(pu => pu.ProjectId == firstProjectId)
                    .ToListAsync(cancellationToken);

                // پیدا کردن مواردی برای حذف (در دیتابیس هستند ولی در لیست جدید نیستند)
                // اما فقط آنهایی که IsCreator == false دارند
                var toDelete = existingProjectUsers
                    .Where(epu => !projectUsers.Any(npu =>
                        npu.ProjectId == epu.ProjectId && npu.UserId == epu.UserId)
                        && epu.IsCreator == false) // فقط مواردی که IsCreator false هستند
                    .ToList();

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var toAdd = projectUsers
                    .Where(npu => !existingProjectUsers.Any(epu =>
                        epu.ProjectId == npu.ProjectId && epu.UserId == npu.UserId))
                    .ToList();

                // اجرای عملیات
                if (toDelete.Any())
                {
                    _context.ProjectUsers.RemoveRange(toDelete);
                }

                if (toAdd.Any())
                {
                    await _context.ProjectUsers.AddRangeAsync(toAdd, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        public async Task DeleteProjectUser(int  projectId, CancellationToken cancellationToken)
        {
            try
            {

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var deleteRoleMenu = _context.ProjectUsers.Where(s => s.ProjectId== projectId)
                    .ToList();
                _context.ProjectUsers.RemoveRange(deleteRoleMenu);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task<bool> HasMenuAccessAsync(string roleId, string menuUrl, CancellationToken cancellationToken)
        {
            var menu = await _context.Menus
                .Where(s => s.Url == menuUrl)
                .FirstOrDefaultAsync(cancellationToken);

            if (menu == null) return false;

            return await _context.RoleMenus
                .AnyAsync(s => s.RoleId.ToString() == roleId && s.MenuId == menu.Id, cancellationToken);
        }
     


        private string GetUserIdManager( string userId)
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
                                    Role=role
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
            if (baseQuery2.Count()==0 )
            {
                return userId;
            }
            if (baseQuery2.First().Role.TypeRole==1)
            {
                return userId;
            }
            else
            {
                var t= baseQuery2.First().User.Id.ToString();
                return t;
            }

        }


    }
}
