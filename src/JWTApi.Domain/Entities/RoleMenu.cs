using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class RoleMenu
    {
        
        public Guid RoleId { get; set; }
        public int MenuId { get; set; }
        //public Guid PermissionId { get; set; }

        public Role Role { get; set; } = default!;
        public Menu Menu { get; set; } = default!;
        //public Permission Permission { get; set; } = default!;
    }
}
