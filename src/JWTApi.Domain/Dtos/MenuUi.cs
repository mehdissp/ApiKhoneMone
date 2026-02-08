using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
   public class MenuUi
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
    }
}
