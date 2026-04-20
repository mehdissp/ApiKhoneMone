using JWTApi.Domain.Shared;
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
        public bool IsDeleted { get; set; } = false;

        // ارتباط با پروفایل مشاور املاک (  // اضافه کردن نقش کاربر به صورت مستقیم
        public UserRoleType Role { get; set; } = UserRoleType.EndUser; // پیش‌فرض فروشنده/خریداراگر مشاور املاک باشد)
        public string CodeMoaref { get; set; }


        // ============== فیلدهای جدید OTP ==============
        public bool IsMobileVerified { get; private set; } = false;
        public DateTime? MobileVerifiedAt { get; private set; }
        public int FailedOtpAttempts { get; private set; } = 0;
        public DateTime? LastOtpRequestTime { get; private set; }
        public DateTime? OtpLockUntil { get; private set; } // قفل درخواست OTP تا زمان مشخص

        public virtual RealEstateAgentProfile? RealEstateAgentProfile { get; set; }

        // ارتباط با مشاور املاکی که این کاربر را ثبت کرده (برای مشاوران مستقل و خریداران)
        public Guid? RegisteredByAgentId { get; set; }
        public virtual User? RegisteredByAgent { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        //public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ExtraProject> ExtraProjects { get; set; } = new List<ExtraProject>();
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        public ICollection<RealEstates> RealEstates { get; set; } = new List<RealEstates>();
  

        

        private User() { }

        public User(string username, string email)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            CreatedAt = DateTime.UtcNow;
            GenerateReferralCode(); // تولید کد معرف به صورت خودکار
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
            CreatedAt = DateTime.UtcNow;
            GenerateReferralCode();

        }
        public User(string userId,string username, string email, bool isActive, string mobileNumber, string fullName)
        {
           
            Username = username;
            Email = email;
            MobileNumber = mobileNumber;

            IsActive = isActive;
            FullName = fullName;
            GenerateReferralCode();

        }
        public void SetPassword(string hash)
        {
            PasswordHash = hash;
        }
        // ============== متدهای کمکی ==============

        private void GenerateReferralCode()
        {
            // تولید کد معرف 8 رقمی
            CodeMoaref = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
        public void UpdateReferralCode(string newCode)
        {
            if (!string.IsNullOrWhiteSpace(newCode))
                CodeMoaref = newCode;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            IncrementTokenVersion(); // باطل کردن توکن‌ها
        }

        public void Restore()
        {
            IsDeleted = false;
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            return RefreshToken == refreshToken &&
                   RefreshTokenExpiryTime.HasValue &&
                   RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
        // ============== متدهای مدیریت OTP ==============

        public bool CanRequestOtp()
        {
            // بررسی قفل بودن
            if (OtpLockUntil.HasValue && OtpLockUntil.Value > DateTime.UtcNow)
                return false;

            // بررسی محدودیت درخواست (حداکثر 3 بار در هر 10 دقیقه)
            if (LastOtpRequestTime.HasValue)
            {
                var minutesSinceLastRequest = (DateTime.UtcNow - LastOtpRequestTime.Value).TotalMinutes;
                if (minutesSinceLastRequest < 1) // حداقل 1 دقیقه بین درخواست‌ها
                    return false;
            }

            return true;
        }

        public (bool success, string message) RecordOtpRequest()
        {
            if (!CanRequestOtp())
                return (false, "لطفاً چند دقیقه دیگر تلاش کنید");

            LastOtpRequestTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            return (true, "کد تایید ارسال شد");
        }

        public void RecordFailedOtpAttempt()
        {
            FailedOtpAttempts++;
            UpdatedAt = DateTime.UtcNow;

            // اگر 5 بار تلاش ناموفق داشت، قفل کردن به مدت 30 دقیقه
            if (FailedOtpAttempts >= 5)
            {
                LockOtpRequestsForMinutes(30);
            }
        }

        public void LockOtpRequestsForMinutes(int minutes)
        {
            OtpLockUntil = DateTime.UtcNow.AddMinutes(minutes);
            UpdatedAt = DateTime.UtcNow;
        }

        public void VerifyMobile()
        {
            IsMobileVerified = true;
            MobileVerifiedAt = DateTime.UtcNow;
            FailedOtpAttempts = 0;
            OtpLockUntil = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ResetOtpAttempts()
        {
            FailedOtpAttempts = 0;
            OtpLockUntil = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
