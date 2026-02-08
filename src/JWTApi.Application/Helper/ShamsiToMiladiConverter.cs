using System;
using System.Globalization;
using System.Text;

public class ShamsiToMiladiConverter
{
    public static DateTime ConvertShamsiToMiladi(string shamsiDate)
    {
        // جدا کردن قسمت‌های تاریخ
        string[] parts = shamsiDate.Split('/');

        if (parts.Length != 3)
            throw new ArgumentException("فرمت تاریخ نامعتبر است");

        int year = int.Parse(ConvertToWesternNumerals(parts[0]));
        int month = int.Parse(ConvertToWesternNumerals(parts[1]));
        int day = int.Parse(ConvertToWesternNumerals(parts[2]));

        // ایجاد شیء PersianCalendar
        PersianCalendar pc = new PersianCalendar();

        // تبدیل به میلادی
        DateTime miladiDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);

        return miladiDate;
    }

    private static string ConvertToWesternNumerals(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder result = new StringBuilder();
        foreach (char c in input)
        {
            // تبدیل اعداد فارسی و عربی به انگلیسی
            if (c >= '۰' && c <= '۹') // اعداد فارسی
                result.Append((char)('0' + (c - '۰')));
            else if (c >= '٠' && c <= '٩') // اعداد عربی
                result.Append((char)('0' + (c - '٠')));
            else
                result.Append(c);
        }
        return result.ToString();
    }
}

// نمونه استفاده
class Program
{
    static void Main()
    {
        string shamsiDate = "1404/05/01";
        DateTime miladiDate = ShamsiToMiladiConverter.ConvertShamsiToMiladi(shamsiDate);

        Console.WriteLine($"تاریخ شمسی: {shamsiDate}");
        Console.WriteLine($"تاریخ میلادی: {miladiDate:yyyy/MM/dd}");
        // خروجی: 2025/07/23
    }
}