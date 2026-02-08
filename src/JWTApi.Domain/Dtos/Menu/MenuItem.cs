using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Menu
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string Path { get; set; }
        public bool? IsMenu { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsCheck { get; set; }
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }
    public class MenuTree
    {
        public List<MenuItem> RootMenus { get; set; } = new List<MenuItem>();
    }
}
