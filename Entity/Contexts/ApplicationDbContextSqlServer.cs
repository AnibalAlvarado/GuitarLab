using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contexts
{
    public class ApplicationDbContextSqlServer : ApplicationDbContext
    {
        public ApplicationDbContextSqlServer(DbContextOptions<ApplicationDbContext> options)
                 : base(options)
        {
        }
    }
}
