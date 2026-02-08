using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
 
    public class ExtraProject
    {
   
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int CountProject { get; set; } // تعداد پروژه اضافه
        public int CountUsers { get; set; } // تعداد پروژه اضافه
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
    }
}
