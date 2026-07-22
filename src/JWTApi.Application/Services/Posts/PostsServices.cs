using JWTApi.Application.DTOs.Posts;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Post;
using JWTApi.Domain.Entities.Blogs;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Payments;
using JWTApi.Domain.Interfaces.Posts;
using JWTApi.Domain.Interfaces.RealEstatesApplications;
using JWTApi.Domain.Interfaces.RealEstateses;
using JWTApi.Domain.Interfaces.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Posts
{
    public class PostsServices
    {

        private readonly IPostsRepository _postsRepository;
        public PostsServices(IPostsRepository postsRepository
            )
        {
            _postsRepository= postsRepository;
        }
        public async Task<List<CategoryPostsDTOS>> GetCategoryPostsDTOs(CancellationToken cancellationToken)
        {
            return await _postsRepository.GetCategoryPostsDTOs(cancellationToken);
        }

        public async Task<List<TagsDtos>> GetTagsDtosDTOs(int catId, CancellationToken cancellationToken)
        {
            return await _postsRepository.GetTagsDtosDTOs(catId, cancellationToken);
        }

        public async Task InsertPost(PostDtos postDtos,string userId,CancellationToken cancellationToken)
        {
            Post post = new Post();
            post.create(postDtos.Title, postDtos.Slug, postDtos.Summary, postDtos.Content, postDtos.ImageUrl, false, userId, (int)postDtos.CategoryId);
            await _postsRepository.InsertPost(post, postDtos.TagsId, cancellationToken);
        }

        public async Task<PagedResult<PostCategoryDto>> getPostCategoryDto(int? categoryId,string searchStream,int pageSize,int pageNumber,CancellationToken cancellationToken)
        {
            return await _postsRepository.GetPostCategoryDto(pageNumber, pageSize, searchStream, categoryId,cancellationToken);
        }
        public async Task<PostDetailsDtos> GetDetailsDtosAsync(int id, CancellationToken cancellationToken)
        {
            return await _postsRepository.GetDetailsDtosAsync(id, cancellationToken);
        }
        public async Task<List<PostCategoryDto>> GetTopViewedPostsAsync(
        CancellationToken cancellationToken)
        {
            return await _postsRepository.GetTopViewedPostsAsync(cancellationToken);
        }
       public async Task<List<PostCategoryDto>> GetTopNewPostsAsync(
      CancellationToken cancellationToken)
        {
            return await _postsRepository.GetTopNewPostsAsync(cancellationToken);
        }

        public async Task UpdateViewCount(int id, CancellationToken cancellationToken)
        {
            await _postsRepository.UpdateViewCount(id, cancellationToken);
        }
        public async Task<PagedResult<PostCategoryDto>> GetPostCategoryDtoAdmin
    (
int pageNumber = 1,
int pageSize = 10,
string searchTerm = null,
int? categoryId = null,
bool? isPublish = true,
CancellationToken cancellationToken = default)
        {
            return await _postsRepository.GetPostCategoryDtoAdmin(pageNumber, pageSize, searchTerm, categoryId, isPublish,cancellationToken);
        }
        public async Task DeleteStatus(int id, CancellationToken cancellationToken)
        {
            await _postsRepository.DeleteStatus(id, cancellationToken);
        }
        public async Task UpdateStatus(int id,bool type, CancellationToken cancellationToken)
        {
            await _postsRepository.UpdateStatus(id, type, cancellationToken);
        }
    }
}
