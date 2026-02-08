using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Roles
{
    public class RoleService
    {
        private readonly IRoleRespository _roleRespository;
        public RoleService(IRoleRespository roleRespository)
        {
            _roleRespository = roleRespository;
        }

        public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken)
        {
            return await _roleRespository.GetRolesForAdmin(cancellationToken);
        }
    }
}
