using JWTApi.Domain.Dtos.Categories;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Categories
{
    public interface ICategoryRepository
    {
        Task Insert(Category category, CancellationToken cancellationToken);
        //Task Update(Category category, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);
        Task<List<Category>> GetAll( CancellationToken cancellationToken);
        Task<Category> GetById(int id, CancellationToken cancellationToken);


        Task<List<CategoryDto>> GetCategoryDtos(int tabId,CancellationToken cancellationToken);
    }
}
