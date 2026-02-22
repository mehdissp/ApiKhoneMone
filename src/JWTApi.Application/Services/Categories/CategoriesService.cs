using JWTApi.Application.DTOs;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Categories;
using JWTApi.Domain.Interfaces.Menus;
using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Categories
{
    public class CategoriesService
    {
        private ICategoryRepository _categoryRepository;
        private IUnitOfWork _unitOfWork;
        public CategoriesService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Category>> GetAllCategoriesAsync(string userId,CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAll(cancellationToken);
        }

        public async Task<Category> GetCategoryAsync(string userId, int id, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetById(id, cancellationToken);

        }
        public async Task Insert(string usereId,InsertCategoryDto insertCategoryDto,CancellationToken cancellationToken)
        {
            var category = new Category(insertCategoryDto.name, insertCategoryDto.icon, insertCategoryDto.desc, (CategoryType)insertCategoryDto.typeCategory);
            await _categoryRepository.Insert(category, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task Delete(string userId,int id,CancellationToken cancellationToken)
        {
            await _categoryRepository.Delete(id, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task Update(string userId, UpdateCategoryDto updateCategoryDto,CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetById(updateCategoryDto.id, cancellationToken);
            category.UpdateCategory(updateCategoryDto.name, updateCategoryDto.icon, updateCategoryDto.desc, (CategoryType)updateCategoryDto.typeCategory);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
