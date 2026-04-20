using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Shared
{
    public enum CategoryType : byte
    {
        MortgageAndRent = 0,
         
        Procurement=1

    }

    // نقش‌ها (فقط برای تعیین نوع کاربر)
    public enum UserRoleType
    {
        Admin=0,
        RealEstateAgent = 1,  // مشاور املاک (دارای پروفایل تخصصی)
        IndependentAgent = 2, // مشاور مستقل
        EndUser = 3   ,       // فروشنده/خریدار
        Manager=4,
        
    }
}
