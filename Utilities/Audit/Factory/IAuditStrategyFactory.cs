using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Strategy;

namespace Utilities.Audit.Factory
{
    public interface IAuditStrategyFactory
    {
        IAuditStrategy[] GetStrategies(); // Puedes retornar múltiples estrategias
    }
}
