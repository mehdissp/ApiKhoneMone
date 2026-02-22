using JWTApi.Api.Response;
using JWTApi.Application.DTOs;
using JWTApi.Application.DTOs.MenuAccess;
using JWTApi.Application.Services.Categories;
using JWTApi.Application.Services.Menus;
using JWTApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private CategoriesService _category;
        public CategoryController(CategoriesService categoriesService)
        {
            _category=categoriesService;
        }
        [HttpGet("GetAllCategoriesAsync")]
        public async Task<IActionResult> GetAllCategoriesAsync( CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var result = await _category.GetAllCategoriesAsync(userId,cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet("GetCategoriesAsync")]
        public async Task<IActionResult> GetCategoriesAsync(int id,CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var result = await _category.GetCategoryAsync(userId, id, cancellationToken);

                return ResponseApi.Ok(result).ToHttpResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory([FromBody] InsertCategoryDto insertCategoryDto,  CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _category.Insert(userId,insertCategoryDto,cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _category.Update(userId, updateCategoryDto, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromBody]  int id, CancellationToken cancellationToken)
        {

            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _category.Delete(userId, id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

    }
}
