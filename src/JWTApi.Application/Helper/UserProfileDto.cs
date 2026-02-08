using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Helper
{
    public class UserProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string? MobileNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Avatar { get; set; }
        public string RoleName { get; set; }

    }
}
