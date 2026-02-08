using JWTApi.Domain.Dtos.Menu;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Menus
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetMenuTreeAsync(string roleId,CancellationToken cancellationToken);
        Task InsertOrDeleteMenuAccess(List<RoleMenu> roleMenus, CancellationToken cancellationToken);
        Task DeleteMenuAccess(string roleId, CancellationToken cancellationToken);
    }
}
