using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Stories
{
    public interface IFileUploadService
    {
        Task<string?> UploadImageAsync(IFormFile? image, int propertyId);
        void DeleteImage(string? imagePath);
    }
}
