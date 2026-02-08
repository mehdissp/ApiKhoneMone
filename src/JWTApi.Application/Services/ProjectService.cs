using JWTApi.Application.DTOs.MenuAccess;
using JWTApi.Application.DTOs.ProjectUsers;

using JWTApi.Domain.Dtos;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services
{
   public class ProjectService
    {
        private readonly IUnitOfWork _unit;
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository, IUnitOfWork unit)
        {
            _projectRepository = projectRepository;
            _unit = unit;
        }

        public async Task InsertProject(string name, string userId, CancellationToken cancellationToken)
        {

             await _projectRepository.AddAsync(name, userId, cancellationToken);
             
        }
        public async Task DeleteProject(int id ,string userId, CancellationToken cancellationToken)
        {
            await _projectRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<(List<ProjectWithPackageInfoDto> Items, int TotalCount, int TotalPages,bool CheckAccess,bool CheckAccessDelete,bool CheckAccessAssigner)> GetProjectsAsync(string userId,string roleId ,  int pageNumber,
             int pageSize, CancellationToken cancellationToken)
        {
            return await _projectRepository.GetProjectsWithPackageInfo(userId, roleId, pageNumber,pageSize, cancellationToken);
        }


        public async Task InsertOrDeleteUserInProject(List<ProjectUserDtos> projectUsers, int projectId, CancellationToken cancellationToken)
        {
            if (projectUsers == null || !projectUsers.Any())
            {
                await _projectRepository.DeleteProjectUser(projectId, cancellationToken);
            }
            else
            {
                var projectUser = projectUsers.Select(pu => new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = Guid.Parse(pu.UserId) // یا TagId اگر پراپرتی نامش متفاوت است
                }).ToList();

                await _projectRepository.InsertOrDeleteUserInProject(projectUser, cancellationToken);
                await _unit.SaveChanges(cancellationToken);
            }


        }



    }
}
