using JWTApi.Application.DTOs.Stoires;
using JWTApi.Domain.Dtos.Stories;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Stories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Stories
{
    public class StoryService
    {
        private readonly IStoryRepository _storyRepository;
        public StoryService(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }

        public async Task<Story> CreateStory(StoryRequest storyRequest,string userId)
        {
            try
            {
                Story story = new Story();
                story.Create(storyRequest.Title, storyRequest.UrlAddress, storyRequest.RealEstatedId, userId, Domain.Shared.StoryStatusEnum.WaitingForAccept);
                return await _storyRepository.AddAsync(story);

            }
            catch (Exception ex)
            {

                throw;
            }
 
        }
        public async Task<bool> DeleteStory(int id,string userId,string roleId)
        {
            return await _storyRepository.DeleteAsync(id,userId,roleId);

        }

        public async Task<List<StoryProfileDto>> StoryProfileDtosAsync(string userId,CancellationToken cancellationToken)
        {
            return await _storyRepository.StoryProfileDtos(userId, cancellationToken);
        }

        public async Task<List<StoryForSite>> StoryForSite( CancellationToken cancellationToken)
        {
            return await _storyRepository.GetStoriesDtos(cancellationToken);
        }

        public async Task<List<StoryForSite>> StoryForSiteForUser(string userId,CancellationToken cancellationToken)
        {
            return await _storyRepository.GetStoriesDtosForUser(userId, cancellationToken);
        }


    }
}
