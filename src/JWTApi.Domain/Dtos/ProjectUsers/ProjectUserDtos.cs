using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.ProjectUsers
{
   public class ProjectUserDtos
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public bool IsCheck { get; set; }
    }
}
