using JWTApi.Domain.Dtos.Post;
using JWTApi.Domain.Entities.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Posts
{
    public interface IPostsRepository
    {
        Task<List<CategoryPostsDTOS>> GetCategoryPostsDTOs(CancellationToken cancellationToken);
        Task<List<TagsDtos>> GetTagsDtosDTOs(int catId, CancellationToken cancellationToken);
        Task InsertPost(Post post, int[] tagIds, CancellationToken cancellationToken);
    }
}
