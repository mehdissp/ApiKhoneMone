using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Package
    {
       
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int MaxProjects { get; set; } // تعداد پروژه‌ای که این پکیج میده
        public int MaxUsers { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
    }
}
