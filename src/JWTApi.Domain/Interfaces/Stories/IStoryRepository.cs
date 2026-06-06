using JWTApi.Domain.Dtos.Stories;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Stories
{
    // Repositories/IStoryRepository.cs




    public interface IStoryRepository : IDisposable
    {
        // Get
        Task<Story?> GetByIdAsync(int id);
        Task<IEnumerable<Story>> GetActiveStoriesByPropertyAsync(int propertyId);
        Task<IEnumerable<Story>> GetAllExpiredStoriesAsync();
        Task<bool> StoryExistsAsync(int id);
        //Task<List<StoriesDtos>> GetStoriesDtos(CancellationToken cancellationToken);

        // Insert
        Task<Story> AddAsync(Story story);
        Task AddRangeAsync(IEnumerable<Story> stories);

        // Delete
        Task<bool> DeleteAsync(int id, string userId, string roleName);
        Task<int> DeleteExpiredStoriesAsync();
        Task<int> DeleteByPropertyAsync(int propertyId);

        // Save
        Task<int> SaveChangesAsync();
        Task<List<StoryProfileDto>> StoryProfileDtos(string userId, CancellationToken cancellationToken);

        Task<List<StoryForSite>> GetStoriesDtos(CancellationToken cancellationToken);
    }
}
