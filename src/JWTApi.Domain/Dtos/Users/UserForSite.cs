using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Users
{
    public class UserForSite
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Score { get; set; }
        public int CountOfRealEtates { get; set; }
        public int CountOfRent { get; set; }
        public string DateTimeOfSite { get; set; }
        public string[] RegionOfWork { get; set; }
        public string Avatar { get; set; }
    }
}
