using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
    public class MenuPermissionDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = default!;
        public string? Url { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
