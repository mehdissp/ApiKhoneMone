using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.ImageInfos
{
    public class TempImage
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime ExpireAt { get; set; }
        public string OriginalName { get; set; }
    }
    // کلاس اطلاعات تصویر برای سئو
    public class ImagesInfo
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public double SizeKB { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string AltText { get; set; }
        public string Title { get; set; }

        // تولید تگ تصویر کامل برای استفاده در View
        public string ToImageTag(string additionalClasses = "")
        {
            return $"<img src='{Url}' alt='{AltText}' title='{Title}' width='{Width}' height='{Height}' class='{additionalClasses}' loading='lazy' />";
        }

        // تولید Schema markup برای سئو پیشرفته
        public string ToSchemaMarkup()
        {
            return $@"
        <script type='application/ld+json'>
        {{
            ""@context"": ""https://schema.org/"",
            ""@type"": ""ImageObject"",
            ""contentUrl"": ""{Url}"",
            ""name"": ""{Title}"",
            ""description"": ""{AltText}"",
            ""width"": ""{Width}"",
            ""height"": ""{Height}""
        }}
        </script>";
        }
    }
}
