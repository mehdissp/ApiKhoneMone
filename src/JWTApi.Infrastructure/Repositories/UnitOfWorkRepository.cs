using JWTApi.Domain.Dtos;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWorkRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task SaveChanges(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task CheckAccess(string roleId, string userId,CancellationToken cancellationToken)
        {
            var userGuid = Guid.Parse(userId);
            
            //var check= await _context.Todos.AnyAsync(s => s.Id == todoId && (s.UserId == userGuid || s.UserTodo == userGuid), cancellationToken);
            //if (check==false)
            //{
                throw new RestBasedException(ApiErrorCodeMessage.Error_Access);
            //}

        }
    }
}
