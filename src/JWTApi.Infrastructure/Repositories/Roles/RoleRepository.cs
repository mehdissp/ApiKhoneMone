using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Roles;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Roles
{
    public class RoleRepository : IRoleRespository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<List<Role>> GetRolesForAdmin(CancellationToken cancellationToken)
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
