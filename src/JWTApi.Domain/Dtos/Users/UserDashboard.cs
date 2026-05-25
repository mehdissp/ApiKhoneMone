using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Users
{
    public class UserDashboard
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public decimal? WalletBalance { get; set; }
        public string Rating { get; set; }
        public string Mobile { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string Experience { get; set; }
        public string TotalSessions { get; set; }
        public string Specialty { get; set; }


 
    }
}
