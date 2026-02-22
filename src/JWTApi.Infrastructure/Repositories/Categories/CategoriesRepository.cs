using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Categories;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Categories
{
    public class CategoriesRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoriesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var category = await GetById(id, cancellationToken);
            if (category == null)
                throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);

            if (await CheckInRef(category, cancellationToken))
                throw new RestBasedException(ApiErrorCodeMessage.Error_Accessapi);

            category.IsDeleted = true;
      
        }

        public async Task<List<Category>> GetAll(CancellationToken cancellationToken)
        {
           return await _context.Categories.ToListAsync(cancellationToken);
        }

        public async Task<Category> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.Categories.FindAsync(id, cancellationToken);
        }

        public async Task<List<CategoryDto>> GetCategoryDtos(int tabId,CancellationToken cancellationToken)
        {
            return await _context.Categories
          .Where(s => s.CategoryType == (CategoryType)tabId)
          .Select(s => new CategoryDto
          {
              Id = s.Id,
              Name = s.Name,        // یا s.Title بستگی به نام فیلد در مدل Category شما دارد
              Icon = s.Icon
          })
          .ToListAsync(cancellationToken);
        }

        public async Task Insert(Category category, CancellationToken cancellationToken)
        {
          await _context.Categories.AddAsync(category, cancellationToken);
        }

    
        private async Task<bool> CheckInRef(Category category, CancellationToken cancellationToken)
        {
            var existsInRealEstates = await _context.RealEstates
                .Where(s => s.CategoryId == category.Id)
                .Select(s => s.Id)
                .Take(1)
                .Union(_context.RealEstatesRents
                    .Where(s => s.CategoryId == category.Id)
                    .Select(s => s.Id)
                    .Take(1))
                .AnyAsync(cancellationToken);

            return existsInRealEstates;
        }
    }
}
