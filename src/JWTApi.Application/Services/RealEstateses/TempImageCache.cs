using JWTApi.Domain.Dtos.ImageInfos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace JWTApi.Application.Services.RealEstateses
{
    public class TempImageCache
    {
        private readonly Dictionary<string, TempImage> _cache = new();
        private readonly IWebHostEnvironment _env;

        public TempImageCache(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveToCache(IFormFile image)
        {
            var cacheId = Guid.NewGuid().ToString();
            var cacheFolder = Path.Combine(_env.WebRootPath, "temp", cacheId);

            Directory.CreateDirectory(cacheFolder);

            var ext = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(cacheFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            _cache[cacheId] = new TempImage
            {
                Id = cacheId,
                Path = filePath,
                ExpireAt = DateTime.UtcNow.AddHours(1),
                OriginalName = image.FileName
            };

            return cacheId;
        }

        //public async Task<List<string>> MoveToPermanent(List<string> cacheIds, string currentUserId, int categoryId)
        //{
        //    var permanentUrls = new List<string>();

        //    foreach (var cacheId in cacheIds)
        //    {
        //        Console.WriteLine($"Looking for cacheId: {cacheId}");
        //        Console.WriteLine($"Available keys: {string.Join(", ", _cache.Keys)}");
        //        if (!_cache.ContainsKey(cacheId)) continue;

        //        var temp = _cache[cacheId];

        //        // ساختار پوشه
        //        var userFolder = Path.Combine(_env.WebRootPath, "uploads", "properties", currentUserId, categoryId.ToString());
        //        Directory.CreateDirectory(userFolder);

        //        var fileName = Path.GetFileNameWithoutExtension(temp.Path);
        //        var extension = Path.GetExtension(temp.Path).ToLower();
        //        var newFileName = $"{fileName}_compressed{extension}";
        //        var newPath = Path.Combine(userFolder, newFileName);

        //        // فشرده سازی عکس
        //        await CompressImage(temp.Path, newPath, extension);

        //        // حذف فایل موقت و پوشه
        //        File.Delete(temp.Path);
        //        var dir = Path.GetDirectoryName(temp.Path);
        //        if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
        //            Directory.Delete(dir);

        //        _cache.Remove(cacheId);
        //        permanentUrls.Add($"/uploads/properties/{currentUserId}/{categoryId}/{newFileName}");
        //    }

        //    return permanentUrls;
        //}

        //private async Task CompressImage(string inputPath, string outputPath, string extension)
        //{
        //    using var image = await Image.LoadAsync(inputPath);

        //    // محاسبه ابعاد جدید اگر خیلی بزرگ باشد
        //    var maxDimension = 1920; // حداکثر 1920px برای نمایش در وب
        //    if (image.Width > maxDimension || image.Height > maxDimension)
        //    {
        //        image.Mutate(x => x.Resize(new ResizeOptions
        //        {
        //            Mode = ResizeMode.Max,
        //            Size = new Size(maxDimension, maxDimension)
        //        }));
        //    }

        //    // فشرده سازی بر اساس فرمت
        //    if (extension == ".jpg" || extension == ".jpeg")
        //    {
        //        var encoder = new JpegEncoder
        //        {
        //            Quality = 85, // 85% کیفیت تعادل عالی بین حجم و کیفیت
        //            ColorType = JpegEncodingColor.YCbCrRatio420  // معادل قدیمی Chroma420
        //        };
        //        await image.SaveAsync(outputPath, encoder);
        //    }
        //    else if (extension == ".png")
        //    {
        //        // برای PNG های با شفافیت
        //        var encoder = new PngEncoder
        //        {
        //            CompressionLevel = PngCompressionLevel.BestCompression,
        //            BitDepth = PngBitDepth.Bit8
        //        };
        //        await image.SaveAsync(outputPath, encoder);
        //    }
        //    else
        //    {
        //        // سایر فرمت‌ها بدون تغییر
        //        File.Copy(inputPath, outputPath);
        //    }
        //}

        public async Task<List<ImagesInfo>> MoveToPermanent(List<string> cacheIds, string currentUserId, int categoryId)
        {
            var permanentImages = new List<ImagesInfo>();

            foreach (var cacheId in cacheIds)
            {
                if (!_cache.ContainsKey(cacheId)) continue;

                var temp = _cache[cacheId];

                // ساختار پوشه سئو پسند
                var seoFolderName = CleanForUrl(categoryId.ToString());
                var userFolder = Path.Combine(_env.WebRootPath, "uploads", "properties", seoFolderName, currentUserId);
                Directory.CreateDirectory(userFolder);

                // نام فایل سئو پسند
                var originalName = Path.GetFileNameWithoutExtension(temp.OriginalName);
                var cleanName = CleanForUrl(originalName);
                var extension = Path.GetExtension(temp.Path).ToLower();

                // تولید نام فایل بهینه برای سئو
                var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                var newFileName = $"{cleanName}-{timestamp}-{categoryId}.webp"; // استفاده از WebP
                var newPath = Path.Combine(userFolder, newFileName);

                // فشرده سازی و بهینه سازی عکس
                var imageInfo = await OptimizeImageForSEO(temp.Path, newPath, temp.OriginalName);

                // حذف فایل موقت
                File.Delete(temp.Path);
                var dir = Path.GetDirectoryName(temp.Path);
                if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    Directory.Delete(dir);

                _cache.Remove(cacheId);

                imageInfo.Url = $"/uploads/properties/{seoFolderName}/{currentUserId}/{newFileName}";
                permanentImages.Add(imageInfo);
            }

            return permanentImages;
        }

        private async Task<ImagesInfo> OptimizeImageForSEO(string inputPath, string outputPath, string originalName)
        {
            using var image = await Image.LoadAsync(inputPath);

            // حذف متادیتای اضافی (کاهش حجم و افزایش سرعت)
            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;
            image.Metadata.XmpProfile = null;

            // تعیین ابعاد بهینه بر اساس کاربرد
            var dimensions = GetOptimalDimensions(image.Width, image.Height);

            // تغییر ابعاد اگر لازم باشد
            if (image.Width > dimensions.Width || image.Height > dimensions.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(dimensions.Width, dimensions.Height)
                }));
            }

            // ذخیره با فرمت WebP (بهترین برای سئو)
            var encoder = new WebpEncoder
            {
                Quality = 80, // بهترین تعادل کیفیت/حجم برای سئو
             //   Method = WebpEncodingMethod.Quick,
                FileFormat = WebpFileFormatType.Lossy,
                TransparentColorMode = WebpTransparentColorMode.Preserve
            };

            await image.SaveAsync(outputPath, encoder);

            // محاسبه اطلاعات برای سئو
            var fileInfo = new FileInfo(outputPath);
            var imgInfo = new ImagesInfo
            {
                FileName = Path.GetFileName(outputPath),
                SizeKB = Math.Round(fileInfo.Length / 1024.0, 2),
                Width = image.Width,
                Height = image.Height,
                AltText = GenerateAltText(originalName),
                Title = GenerateTitle(originalName)
            };

            return imgInfo;
        }

        private (int Width, int Height) GetOptimalDimensions(int originalWidth, int originalHeight)
        {
            // ابعاد استاندارد برای نمایش در وب (سئو پسند)
            var maxWidth = 1200;   // حداکثر عرض برای محتوای اصلی
            var maxHeight = 1200;  // حداکثر ارتفاع
            var thumbnailSize = 300; // برای تام‌نیل

            // تشخیص نوع کاربرد (می‌توانید منطق خود را اضافه کنید)
            if (originalWidth <= 300 && originalHeight <= 300)
                return (originalWidth, originalHeight);

            if (originalWidth > maxWidth || originalHeight > maxHeight)
                return (maxWidth, maxHeight);

            return (originalWidth, originalHeight);
        }

        private string CleanForUrl(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "image";

            // تبدیل به حروف کوچک
            text = text.ToLower().Trim();

            // جایگزینی فاصله با خط تیره
            text = Regex.Replace(text, @"\s+", "-");

            // حذف کاراکترهای غیرمجاز (فقط حروف انگلیسی، اعداد، خط تیره و زیرخط)
            text = Regex.Replace(text, @"[^a-z0-9\-_]", "");

            // حذف خط تیره‌های تکراری
            text = Regex.Replace(text, @"-{2,}", "-");

            // محدودیت طول
            if (text.Length > 50)
                text = text.Substring(0, 50);

            return text;
        }

        private string GenerateAltText(string originalName)
        {
            // تولید متن alt بهینه برای سئو
            var cleanName = CleanForUrl(originalName);
            var words = cleanName.Split('-');

            // حذف کلمات اضافی و تکراری
            var importantWords = words
                .Where(w => !string.IsNullOrEmpty(w) && w.Length > 2)
                .Distinct()
                .Take(5);

            var altText = string.Join(" ", importantWords);

            // اضافه کردن کلمات کلیدی کلی
            return $"{altText} | محصول با کیفیت اصلی".Trim();
        }

        private string GenerateTitle(string originalName)
        {
            // تولید title برای تصویر
            var cleanName = CleanForUrl(originalName);
            var words = cleanName.Split('-');
            var title = string.Join(" ", words.Take(7));

            return char.ToUpper(title[0]) + title.Substring(1);
        }
        public void ClearCache(string cacheId)
        {
            if (_cache.ContainsKey(cacheId))
            {
                var temp = _cache[cacheId];
                if (File.Exists(temp.Path))
                    File.Delete(temp.Path);
                var dir = Path.GetDirectoryName(temp.Path);
                if (Directory.Exists(dir))
                    Directory.Delete(dir);
                _cache.Remove(cacheId);
            }
        }
    }


}
