using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty; // مثل "CanEditUser"
        public string Name { get; set; } = string.Empty; // نمایش برای ادمین
        public string? Description { get; set; }

        //public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}
