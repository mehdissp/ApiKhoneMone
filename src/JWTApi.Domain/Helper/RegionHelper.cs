using JWTApi.Domain.Dtos.Regions;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Helper
{

    public static class RegionHelper
    {
        public static List<RegionDtos> GetRegionsWithChildFlag(List<Region> allRegions)
        {
            // مرحله 1: ایجاد Lookup برای گروه‌بندی فرزندان بر اساس ParentId
            // این عملیات O(n) است و یک ساختار فوق سریع برای جستجوی فرزندان ایجاد می‌کند
            ILookup<int?, Region> childrenLookup = allRegions
                .Where(r => r.ParentId.HasValue) // فقط گره‌هایی که ParentId دارند
                .ToLookup(r => r.ParentId); // کلید: ParentId، مقدار: لیست فرزندان

            // مرحله 2: ایجاد HashSet از ParentIdهایی که حداقل یک فرزند دارند
            // این کار بسیار سریع‌تر از Count() > 0 است
            HashSet<int> parentsWithChildren = new HashSet<int>(
                childrenLookup.Select(g => g.Key!.Value) // Key غیر null است چون قبلاً فیلتر کرده‌ایم
            );

            // مرحله 3: ساخت لیست خروجی با پیمایش یکباره (O(n))
            List<RegionDtos> result = new List<RegionDtos>(allRegions.Count);
            foreach (var region in allRegions)
            {
                result.Add(new RegionDtos
                {
                    Id = region.Id,
                    Name = region.Name,
                    Latitude = region.Latitude,
                    Longitude = region.Longitude,
                    // بررسی عضویت در HashSet - O(1)
                    CountChild = parentsWithChildren.Contains(region.Id) ? 1 : 0
                });
            }

            return result;
        }
    }
}
