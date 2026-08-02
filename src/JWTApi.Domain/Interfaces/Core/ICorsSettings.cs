using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Core
{
    public interface ICorsSettings
    {
        string[] GetAllowedOrigins();
    }
}
