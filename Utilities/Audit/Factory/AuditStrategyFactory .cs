using Entity.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Strategy;

namespace Utilities.Audit.Factory
{
    public class AuditStrategyFactory : IAuditStrategyFactory
    {
        private readonly AuditDbContextFactory _auditDbFactory;

        public AuditStrategyFactory(AuditDbContextFactory auditDbFactory)
        {
            _auditDbFactory = auditDbFactory;
        }

        public IAuditStrategy[] GetStrategies()
        {
            var dbStrategy = new DatabaseAuditStrategy(_auditDbFactory.Create());
            var fileStrategy = new FileAuditStrategy("Logs/audit.txt");

            return new IAuditStrategy[] { dbStrategy, fileStrategy };
        }
    }
}
