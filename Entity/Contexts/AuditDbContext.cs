using Entity.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Context
{
    public class AuditDbContext : DbContext
    {

        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) 
        { 
            
        }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }

}
