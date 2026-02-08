using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
  public  class Attachment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
