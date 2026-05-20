using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using ImageSharpImage = SixLabors.ImageSharp.Image;

public static class WatermarkHelper
{
    private static IWebHostEnvironment _env;

    // متد برای تنظیم environment (از Program.cs یا Startup.cs صدا زده می‌شود)
    public static void Configure(IWebHostEnvironment env)
    {
        _env = env;
    }

    //public static async Task<byte[]> AddTextWatermark(IFormFile image, string watermarkText, float opacity = 0.6f)
    //{
    //    using (var ms = new MemoryStream())
    //    {
    //        await image.CopyToAsync(ms);
    //        ms.Position = 0;

    //        using (var img = await ImageSharpImage.LoadAsync(ms))
    //        {
    //            try
    //            {
    //                // استفاده از Generic Sans Serif به جای Arial (موجود در همه سیستم‌ها)
    //              //  var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts", "B-NAZANIN.TTF");

    //                // اگر فایل وجود نداشت، مسیر جایگزین
    //                string fontPath = null;

    //                if (_env != null)
    //                {
    //                    fontPath = Path.Combine(_env.WebRootPath, "fonts", "B-NAZANIN.TTF");
    //                }
    //                else
    //                {
    //                    // راه جایگزین اگر محیط در دسترس نبود
    //                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    //                    fontPath = Path.Combine(wwwrootPath, "fonts", "B-NAZANIN.TTF");
    //                }

    //                // بررسی وجود فایل
    //                if (!File.Exists(fontPath))
    //                {
    //                    // امتحان با پسوند .TTF
    //                    fontPath = Path.Combine(Path.GetDirectoryName(fontPath), "B-NAZANIN.TTF");
    //                }

    //                if (!File.Exists(fontPath))
    //                {
    //                    throw new FileNotFoundException($"Font file not found at: {fontPath}");
    //                }

    //                // لود فونت از فایل
    //                var fontCollection = new FontCollection();
    //                var fontFamily = fontCollection.Add(fontPath);
    //                var font = fontFamily.CreateFont(48, FontStyle.Bold);

    //                // اضافه کردن متن در مرکز تصویر
    //                img.Mutate(ctx => ctx.DrawText(
    //                    watermarkText,
    //                    font,
    //                    Color.White.WithAlpha(opacity),
    //                    new PointF(img.Width / 2, img.Height / 2)));

    //                using (var outputMs = new MemoryStream())
    //                {
    //                    await img.SaveAsPngAsync(outputMs);
    //                    return outputMs.ToArray();
    //                }
    //            }
    //            catch (Exception ex)
    //            {

    //                throw;
    //            }

    //        }
    //    }
    //}
    public static async Task<byte[]> AddTextWatermark(IFormFile image, string watermarkText, float opacity = 0.6f)
    {
        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms);
            ms.Position = 0;

            using (var img = await ImageSharpImage.LoadAsync(ms))
            {
                try
                {
                    string fontPath = null;

                    if (_env != null)
                        fontPath = Path.Combine(_env.WebRootPath, "fonts", "B-NAZANIN.TTF");
                    else
                    {
                        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        fontPath = Path.Combine(wwwrootPath, "fonts", "B-NAZANIN.TTF");
                    }

                    if (!File.Exists(fontPath))
                        fontPath = Path.Combine(Path.GetDirectoryName(fontPath), "B-NAZANIN.TTF");

                    if (!File.Exists(fontPath))
                        throw new FileNotFoundException($"Font file not found at: {fontPath}");

                    var fontCollection = new FontCollection();
                    var fontFamily = fontCollection.Add(fontPath);
                    // سایز فونت رو بر اساس عرض تصویر تنظیم کنید (حدود 5% عرض تصویر)
                    float fontSize = Math.Max(24, img.Width / 20);
                    var font = fontFamily.CreateFont(fontSize, FontStyle.Bold);

                    // اندازه متن رو برای وسط‌چین کردن دقیق محاسبه کنید
                    var textOptions = new TextOptions(font);
                    var textSize = TextMeasurer.MeasureAdvance(watermarkText, textOptions);
                    float textX = (img.Width - textSize.Width) / 2;
                    float textY = (img.Height - textSize.Height) / 2;

                    // 1. یه نیمه شفاف (پس‌زمینه) دور متن برای خواناتر شدن
                    float padding = 20;
                    float bgX = textX - padding;
                    float bgY = textY - padding;
                    float bgWidth = textSize.Width + padding * 2;
                    float bgHeight = textSize.Height + padding * 2;

                    img.Mutate(ctx => ctx.Fill(
                        Color.Black.WithAlpha(0.3f),
                        new RectangleF(bgX, bgY, bgWidth, bgHeight)));

                    // 2. سایه متن (با جابجایی 2 پیکسلی)
                    img.Mutate(ctx => ctx.DrawText(
                        watermarkText,
                        font,
                        Color.Black.WithAlpha(0.5f),
                        new PointF(textX + 2, textY + 2)));

                    // 3. متن اصلی سفید با شفافیت دلخواه
                    img.Mutate(ctx => ctx.DrawText(
                        watermarkText,
                        font,
                        Color.White.WithAlpha(opacity),
                        new PointF(textX, textY)));

                    using (var outputMs = new MemoryStream())
                    {
                        await img.SaveAsPngAsync(outputMs);
                        return outputMs.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}