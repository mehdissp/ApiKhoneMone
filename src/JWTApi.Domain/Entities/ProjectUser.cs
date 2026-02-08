using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
  public  class ProjectUser
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProjectId { get; set; }
        public bool IsDeactived { get; set; } = false;
        public bool IsCreator { get; set; } = false;


    }
}
