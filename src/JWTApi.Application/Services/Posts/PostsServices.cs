using JWTApi.Application.DTOs.Posts;
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
            post.create(postDtos.Title, postDtos.Slug, postDtos.Summary, postDtos.Content, postDtos.TempImageCacheIds, false, userId);
            await _postsRepository.InsertPost(post, postDtos.TagsId, cancellationToken);
        }

    }
}
