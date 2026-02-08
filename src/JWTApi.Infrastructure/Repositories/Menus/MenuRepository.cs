using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JWTApi.Infrastructure.Repositories.Menus
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        #region [ - get,menuaccess - ]

        public async Task<List<MenuItem>> GetMenuTreeAsync(string roleId, CancellationToken cancellationToken)
        {
            // خواندن همه منوها بدون Include (برای جلوگیری از circular reference)
            var allMenus = await _context.Menus.Where(s => s.IsDefault != true)
                .AsNoTracking() // برای عملکرد بهتر

                .ToListAsync(cancellationToken);


            // خواندن منوهای مرتبط با نقش
            var roleMenuIds = await _context.RoleMenus
                .Where(rm => rm.RoleId.ToString() == roleId)
                .Select(rm => rm.MenuId)
                .ToListAsync(cancellationToken);


            return BuildTree(allMenus, roleMenuIds);
        }

        private List<MenuItem> BuildTree(List<Menu> allMenus, List<int> roleMenuIds)
        {
            // تبدیل به MenuItem و ساخت درخت
            var menuItems = allMenus.Select(m => new MenuItem
            {
                Id = m.Id,
                Name = m.Name,
                ParentId = m.ParentId,
                Url = m.Url,
                IsCheck = roleMenuIds.Contains(m.Id), // بررسی وجود در RoleMenu
                Children = new List<MenuItem>()
            }).ToList();

            var lookup = menuItems.ToDictionary(m => m.Id);
            var rootMenus = new List<MenuItem>();

            foreach (var menuItem in menuItems)
            {
                if (menuItem.ParentId.HasValue && lookup.ContainsKey(menuItem.ParentId.Value))
                {
                    lookup[menuItem.ParentId.Value].Children.Add(menuItem);
                }
                else
                {
                    rootMenus.Add(menuItem);
                }
            }

            return rootMenus;
        } 
        #endregion


        public async Task InsertOrDeleteMenuAccess(List<RoleMenu> roleMenus,CancellationToken cancellationToken)
        {
            try
            {
                // گرفتن اولین RoleId برای فیلتر کردن (فرض می‌کنیم همه آیتم‌ها RoleId یکسان دارند)
                var firstRoleId = roleMenus.First().RoleId;

                // موجودی فعلی از دیتابیس
                var existingRoleMenus = await _context.RoleMenus
                    .Where(rm => rm.RoleId == firstRoleId)
                    .ToListAsync(cancellationToken);

                // پیدا کردن مواردی برای حذف (در دیتابیس هستند ولی در لیست جدید نیستند)
                var toDelete = existingRoleMenus
                    .Where(erm => !roleMenus.Any(nrm =>
                        nrm.RoleId == erm.RoleId && nrm.MenuId == erm.MenuId))
                    .ToList();

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var toAdd = roleMenus
                    .Where(nrm => !existingRoleMenus.Any(erm =>
                        erm.RoleId == nrm.RoleId && erm.MenuId == nrm.MenuId))
                    .ToList();

                // اجرای عملیات
                if (toDelete.Any())
                {
                   
                    _context.RoleMenus.RemoveRange(toDelete);
                }

                if (toAdd.Any())
                {
                    await _context.RoleMenus.AddRangeAsync(toAdd, cancellationToken);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
       
        }

        public async Task DeleteMenuAccess(string roleId, CancellationToken cancellationToken)
        {
            try
            {

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var deleteRoleMenu = _context.RoleMenus.Where(s=>s.RoleId.ToString()== roleId)
                    .ToList();
                    _context.RoleMenus.RemoveRange(deleteRoleMenu);
               
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}