using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Extentions
{
    public static class Link
    {
        public static string EncodeUrlPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            var parts = path.Split('/');
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = Uri.EscapeDataString(parts[i]);
            }
            return string.Join("/", parts);
        }
    }
}
