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
    public enum DocumentTypeEnum
    {
        SingleLeaf=0,
        Tassel=1,
        Promise=2
    }
    public static class DocumentTypeEnumEnumExtensions
    {
        public static string ToPersianString(this DocumentTypeEnum inputData)
        {
            return inputData switch
            {
                DocumentTypeEnum.SingleLeaf => "تک برگ",
                DocumentTypeEnum.Tassel => "منقوله دار",
                DocumentTypeEnum.Promise => "قولنامه ای",

                _ => "نامشخص",
            };
        }
    }

    public enum RealEstateStatusEnum
    {
        WaitingForAccept = 0,
        WaitingForPayment = 1,
        Accept = 2,  // مشاور املاک (دارای پروفایل تخصصی)
        Reject = 3, // مشاور مستقل
        Archive = 4,       // فروشنده/خریدار
        Sales = 5,

    }
    public static class RealEstateStatusEnumExtensions
    {
        public static string ToPersianString(this RealEstateStatusEnum inputData)
        {
            return inputData switch
            {
                RealEstateStatusEnum.WaitingForAccept => "انتظار",
                RealEstateStatusEnum.WaitingForPayment => "در انتظارپرداخت",
                RealEstateStatusEnum.Accept => "منتشر شد",
                RealEstateStatusEnum.Reject => "رد شد",
                RealEstateStatusEnum.Archive => "بایگانی شد",
                RealEstateStatusEnum.Sales => "فروخته شد",
                _ => "نامشخص",
            };
        }
    }
    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
