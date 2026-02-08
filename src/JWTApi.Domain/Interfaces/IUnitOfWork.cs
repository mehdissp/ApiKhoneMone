using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
  public  interface IUnitOfWork
    {
        Task SaveChanges(CancellationToken cancellationToken);
        Task CheckAccess(int todoId, string userId, CancellationToken cancellationToken);
    }
}
