using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
    public static class ApiErrorCodeMessage
    {


        //----------------------------------------------------------------------------------------------------

 

        /// <summary>
        /// 4484 -  ویژگی درخواست و مدیریت سفته از مشتریان برای پذیرنده فعال نیست.
        /// </summary>
        public static ErrorCodeDto Error_Refrence = ErrorCodeDto.Create(403, "این رکورد داری رفرنس می باشد ");
        public static ErrorCodeDto Error_NotFound = ErrorCodeDto.Create(403, "رکوردی یافت نشد ");
        public static ErrorCodeDto Error_Dublicate = ErrorCodeDto.Create(400, "شماره موبایل تکراری می باشد");
        public static ErrorCodeDto Error_Access = ErrorCodeDto.Create(400, "شما دسترسی پاک کردن این داده رو ندارید");


    }
}
