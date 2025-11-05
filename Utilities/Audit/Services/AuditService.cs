using Entity.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Factory;
using Utilities.Audit.Strategy;

namespace Utilities.Audit.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditStrategyFactory _strategyFactory;

        public AuditService(IAuditStrategyFactory strategyFactory)
        {
            _strategyFactory = strategyFactory;
        }

        public async Task SaveAuditAsync(AuditLog entry)
        {
            var strategies = _strategyFactory.GetStrategies();

            foreach (var strategy in strategies)
            {
                await strategy.AuditAsync(entry);
            }
        }
    }
}
