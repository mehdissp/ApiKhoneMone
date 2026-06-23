using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Violation
    {
        public int Id { get; set; }
        public int RealEstatesId { get; set; }
        public Guid UserId { get; set; }
        public ViolationTypeEnum ViolationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DescriptionRows { get; set; }
        public bool IsDeleted { get; set; } = false;

        public RealEstates RealEstates { get; set; } = default!;

        public void Create(string userId, int realEstatesId,string desc,int errorType)
        {
            // تبدیل string به Guid
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid userId format", nameof(userId));
            }

            UserId = parsedUserId;

            RealEstatesId = realEstatesId;
            CreatedAt = DateTime.Now;
            DescriptionRows=desc;
            ViolationType = (ViolationTypeEnum)errorType;

        }
    }
}
