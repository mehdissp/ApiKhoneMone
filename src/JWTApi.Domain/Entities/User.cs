using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; }
        public int TokenVersion { get; set; } = 1; // فیلد جدید برای نسخه‌بندی توکن
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        //public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ExtraProject> ExtraProjects { get; set; } = new List<ExtraProject>();
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        private User() { }

        public User(string username, string email)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
        }
        public void UpdateUserInfo(string fullName, string userName, string mobileNumber, bool isActive)
        {
            Username = userName;
            FullName = fullName;
            MobileNumber = mobileNumber;
            // اگر کاربر از فعال به غیرفعال تغییر کند
            if (IsActive && !isActive)
            {
                IncrementTokenVersion(); // باطل کردن تمام توکن‌ها
            }

            IsActive = isActive;
        }
        // متد برای افزایش نسخه توکن (باطل کردن تمام توکن‌ها)
        public void IncrementTokenVersion()
        {
            TokenVersion++;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateUserProfile(string avatar)
        {
            Avatar = avatar;
        }

        // متد برای تغییر رمز عبور
        public void ChangePassword(string passwordHash)
        {
            if (!string.IsNullOrEmpty(passwordHash))
            {
                PasswordHash = passwordHash;
            }
        }
        public User(string username, string email, bool isActive,string mobileNumber,string fullName)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            MobileNumber = mobileNumber;
            IsActive = isActive;
            FullName = fullName;

        }
        public User(string userId,string username, string email, bool isActive, string mobileNumber, string fullName)
        {
           
            Username = username;
            Email = email;
            MobileNumber = mobileNumber;

            IsActive = isActive;
            FullName = fullName;

        }
        public void SetPassword(string hash)
        {
            PasswordHash = hash;
        }
    }
}
