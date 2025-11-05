using Entity.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Audit.Services
{
    public interface IAuditService
    {
        Task SaveAuditAsync(AuditLog entry);
    }
}
