using Entity.Context;
using Entity.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Audit.Strategy
{
    public class DatabaseAuditStrategy : IAuditStrategy
    {
        private readonly AuditDbContext _context;

        public DatabaseAuditStrategy(AuditDbContext context)
        {
            _context = context;
        }

        public async Task AuditAsync(AuditLog entry)
        {
            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}
