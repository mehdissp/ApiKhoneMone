using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.ProjectUsers;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user );
        Task<User?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<MenuPermissionDto>> GetUserMenuPermissionsAsync(string userId,CancellationToken cancellationToken);
         Task AddLoginAttemptAsync(LoginAttempt loginAttempt, CancellationToken cancellationToken);
        Task CheckAndLockIp(string? ip);
        Task<List<MenuUi>> GetUserMenuPermissionsForUiAsync(string userId,string roleId, CancellationToken cancellationToken);
        Task AddUserWithAnotherUsers(User user, string userId,string roleId, CancellationToken cancellationToken);
        Task<PagedResult<User>> GetUsersAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<bool> checkUserNameDublicated(string userName, CancellationToken cancellationToken);
        Task<bool> checkMobileDublicated(string mobileNumber, CancellationToken cancellationToken);
        Task<bool> checkUserNameDublicatedUpdate(string userName, string userId, CancellationToken cancellationToken);
        Task<bool> checkMobileDublicatedUpdate(string mobileNumber, string userId, CancellationToken cancellationToken);
        Task<List<Role>> GetRoleCombo(CancellationToken cancellationToken);

        Task<PagedResult<ProjectUserDtos>> GetProjectUserDtos(string userId, int projectId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task EditRole(UserRole userRole, CancellationToken cancellationToken);
        Task<User?> GetByUserIdAsyncForToken(string userId);

        Task<PagedResult<User>> GetUsersForComboAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);


    }
}
