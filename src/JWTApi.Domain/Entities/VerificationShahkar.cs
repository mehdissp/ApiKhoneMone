using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class VerificationShahkar
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public bool Status { get; set; }
        public DateTime TimeRequest { get; set; }
        public decimal? Cost { get; set; }
        public decimal? FeeReceived { get; set; }
        public User User { get; set; } = default!;

        public void create(string userId,string mobilenumber,string nation,bool status,decimal? cost,decimal? feeRecevide)
        {
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid userId format", nameof(userId));
            }

            UserId = parsedUserId;
            MobileNumber =mobilenumber;
            NationalCode = nation;  
            Status = status;
            Cost = cost;
            FeeReceived = feeRecevide;
            TimeRequest = DateTime.Now; 

        }
    }
}
