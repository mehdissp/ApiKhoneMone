using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Users
{
    public class IndependentAgentDtos
    {
        public string FullName { get; set; }
        public string UserId { get; set; }
        public int  TotalRealEstate { get; set; }
        public string Score { get; set; }
        public string Avatar { get; set; }
    }
}
