using JWTApi.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Extentions
{
    public static class AppDbContextExtensions
    {
        public static IDbConnection CreateConnection(this AppDbContext context)
        {
            var connectionString = context.Database.GetConnectionString();
            return new SqlConnection(connectionString);
        }
    }
}
