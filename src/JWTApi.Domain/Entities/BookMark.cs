using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class BookMark
    {
        public int Id { get; set; }
        public int RealEstatesId { get; set; }
        public Guid UserId{ get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DescriptionRows { get; set; }

        public RealEstates RealEstates { get; set; } = default!;

        public void Create(string userId,int realEstatesId)
        {
            // تبدیل string به Guid
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid userId format", nameof(userId));
            }

            UserId = parsedUserId;

            RealEstatesId = realEstatesId;
            CreatedAt = DateTime.Now;
        }
    }
}
