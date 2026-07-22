using JWTApi.Domain.Dtos;
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
        Task<PagedResult<PostCategoryDto>> GetPostCategoryDto
            (
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default);
        Task<PostDetailsDtos> GetDetailsDtosAsync(int id, CancellationToken cancellationToken);

        Task<List<PostCategoryDto>> GetTopViewedPostsAsync(CancellationToken cancellation);
         Task<List<PostCategoryDto>> GetTopNewPostsAsync(
      CancellationToken cancellationToken);

        Task UpdateViewCount(int id, CancellationToken cancellationToken);
        Task<PagedResult<PostCategoryDto>> GetPostCategoryDtoAdmin
    (
int pageNumber = 1,
int pageSize = 10,
string searchTerm = null,
int? categoryId = null,
bool? isPublish = true,
CancellationToken cancellationToken = default);

        Task UpdateStatus(int id, bool type, CancellationToken cancellationToken);
        Task DeleteStatus(int id, CancellationToken cancellationToken);
    }
}
