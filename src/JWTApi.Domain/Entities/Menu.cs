using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Path { get; set; }
        public string? Label { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; } // مثل /api/users یا /admin/dashboard
        //public string? Controller { get; set; }
        //public string? Action { get; set; }
        public bool? IsMenu { get; set; }
        public bool IsDefault { get; set; } = false;

        public int? ParentId { get; set; }
        public Menu? Parent { get; set; }
        public ICollection<Menu> Children { get; set; } = new List<Menu>();
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}
