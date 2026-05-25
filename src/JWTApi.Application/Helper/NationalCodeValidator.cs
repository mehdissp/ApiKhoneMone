using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JWTApi.Application.Helper
{
    public class NationalCodeValidator
    {
        public static bool IsValidNationalCode(string nationalCode)
        {
            // 1. ورودی نباید null یا خالی باشد
            if (string.IsNullOrWhiteSpace(nationalCode))
                return false;

            // 2. حذف فضاهای خالی اضافی
            nationalCode = nationalCode.Trim();

            // 3. بررسی فرمت عددی (فقط رقم)
            if (!Regex.IsMatch(nationalCode, @"^\d{10}$"))
                return false;

            // 4. کدهای ملی ساختگی معروف (تمام رقم تکراری)
            string[] invalidPatterns =
            {
            "0000000000", "1111111111", "2222222222", "3333333333",
            "4444444444", "5555555555", "6666666666", "7777777777",
            "8888888888", "9999999999"
        };

            if (invalidPatterns.Contains(nationalCode))
                return false;

            // 5. محاسبه رقم کنترل
            int checkDigit = int.Parse(nationalCode[9].ToString());
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(nationalCode[i].ToString());
                sum += digit * (10 - i);
            }

            int remainder = sum % 11;

            // 6. بررسی نهایی رقم کنترل
            if (remainder < 2)
                return checkDigit == remainder;
            else
                return checkDigit == 11 - remainder;
        }
    }
}
