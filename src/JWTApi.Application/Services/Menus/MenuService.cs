using JWTApi.Application.DTOs.MenuAccess;

using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Menus
{
   public class MenuService
    {
        private IMenuRepository _menuRepository;
        private IUnitOfWork _unitOfWork;
        public MenuService(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MenuItem>> MenuItemsAsync(string roleId,CancellationToken cancellationToken)
        {
            return await _menuRepository.GetMenuTreeAsync(roleId, cancellationToken);
        }

        public async Task InsertOrDeleteMenuAccess(List<MenuAccessDtos> menuAccessDtos,string roleId,CancellationToken cancellationToken)
        {
            if (menuAccessDtos == null || !menuAccessDtos.Any())
            {
                await _menuRepository.DeleteMenuAccess(roleId, cancellationToken);
            }
            else
            {
                var menuAccess = menuAccessDtos.Select(menuac => new RoleMenu
                {
                    MenuId = (int)menuac.MenuId,
                    RoleId = Guid.Parse(roleId) // یا TagId اگر پراپرتی نامش متفاوت است
                }).ToList();

                await _menuRepository.InsertOrDeleteMenuAccess(menuAccess, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
 

        }
    }
}
