using Entity.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Audit.Strategy
{
    public class CombinedAuditStrategy : IAuditStrategy
    {
        private readonly IEnumerable<IAuditStrategy> _strategies;

        public CombinedAuditStrategy(IEnumerable<IAuditStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task AuditAsync(AuditLog entry)
        {
            foreach (var strategy in _strategies)
            {
                await strategy.AuditAsync(entry);
            }
        }
    }
}
