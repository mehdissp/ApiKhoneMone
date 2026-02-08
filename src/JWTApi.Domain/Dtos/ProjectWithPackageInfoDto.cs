using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
    public class ProjectWithPackageInfoDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaxProjects { get; set; }
        public int RowNum { get; set; }

        // اضافه کردن این خصوصیات برای اطلاعات صفحه‌بندی
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public bool CheckAccess { get; set; }
        public bool CheckAccessDelete { get; set; }
        public bool CheckAccessAssigner { get; set; }
    }
}
