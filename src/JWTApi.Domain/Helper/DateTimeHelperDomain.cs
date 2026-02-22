using System;
using System.Globalization;
using System.Threading;

public static class DateTimeHelperDomain
{
    private static readonly PersianCalendar PersianCalendar = new PersianCalendar();

    /// <summary>
    /// تبدیل تاریخ میلادی به شمسی با فرمت زمان نسبی
    /// </summary>
    public static string ToPersianRelativeDate(this DateTime dateTime)
    {
        var now = DateTime.Now;
        var diff = now - dateTime;

        // چند ثانیه پیش
        if (diff.TotalSeconds < 60)
        {
            return "چند ثانیه پیش";
        }

        // چند دقیقه پیش (تا 1 ساعت)
        if (diff.TotalMinutes < 60)
        {
            int minutes = (int)diff.TotalMinutes;
            return $"{minutes} دقیقه پیش";
        }

        // چند ساعت پیش (تا 24 ساعت)
        if (diff.TotalHours < 24)
        {
            int hours = (int)diff.TotalHours;
            return $"{hours} ساعت پیش";
        }

        // چند روز پیش (تا 30 روز)
        if (diff.TotalDays < 30)
        {
            int days = (int)diff.TotalDays;
            return $"{days} روز پیش";
        }

        // چند ماه پیش (تا 12 ماه)
        if (diff.TotalDays < 365)
        {
            int months = (int)(diff.TotalDays / 30);
            return $"{months} ماه پیش";
        }

        // بیشتر از یک سال
        int years = (int)(diff.TotalDays / 365);
        return $"{years} سال پیش";
    }

    /// <summary>
    /// تبدیل تاریخ میلادی به شمسی با فرمت کامل
    /// </summary>
    public static string ToPersianDate(this DateTime dateTime)
    {
        int year = PersianCalendar.GetYear(dateTime);
        int month = PersianCalendar.GetMonth(dateTime);
        int day = PersianCalendar.GetDayOfMonth(dateTime);

        return $"{year}/{month:D2}/{day:D2}";
    }

    /// <summary>
    /// تبدیل تاریخ میلادی به شمسی با فرمت کامل به همراه نام ماه
    /// </summary>
    public static string ToPersianDateString(this DateTime dateTime)
    {
        int year = PersianCalendar.GetYear(dateTime);
        int month = PersianCalendar.GetMonth(dateTime);
        int day = PersianCalendar.GetDayOfMonth(dateTime);

        string[] monthNames = {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        return $"{day} {monthNames[month - 1]} {year}";
    }

    /// <summary>
    /// نمایش زمان باقی‌مانده تا یک تاریخ خاص
    /// </summary>
    public static string ToPersianRemainingTime(this DateTime dateTime)
    {
        var now = DateTime.Now;

        if (dateTime < now)
        {
            return ToPersianRelativeDate(dateTime);
        }

        var diff = dateTime - now;

        if (diff.TotalDays > 365)
        {
            int years = (int)(diff.TotalDays / 365);
            return $"{years} سال دیگر";
        }

        if (diff.TotalDays > 30)
        {
            int months = (int)(diff.TotalDays / 30);
            return $"{months} ماه دیگر";
        }

        if (diff.TotalDays > 1)
        {
            int days = (int)diff.TotalDays;
            return $"{days} روز دیگر";
        }

        if (diff.TotalHours > 1)
        {
            int hours = (int)diff.TotalHours;
            return $"{hours} ساعت دیگر";
        }

        if (diff.TotalMinutes > 1)
        {
            int minutes = (int)diff.TotalMinutes;
            return $"{minutes} دقیقه دیگر";
        }

        return "چند لحظه دیگر";
    }
}