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
    public class ApplicationDbContextPostgreSql : ApplicationDbContext
    {
        public ApplicationDbContextPostgreSql(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
