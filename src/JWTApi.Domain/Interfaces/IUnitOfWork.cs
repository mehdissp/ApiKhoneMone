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
        Task CheckAccess(string roleId, string userId, CancellationToken cancellationToken);
    }
}
