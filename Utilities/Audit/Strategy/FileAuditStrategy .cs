using Entity.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Audit.Strategy
{
    public class FileAuditStrategy : IAuditStrategy
    {
        private readonly string _filePath;

        public FileAuditStrategy(string filePath)
        {
            _filePath = filePath;
        }

        public async Task AuditAsync(AuditLog entry)
        {
            var logLine = $"{entry.Timestamp:u} | {entry.UserName} | {entry.Action} | {entry.EntityName} | ID: {entry.EntityId} | {entry.Changes}";
            await File.AppendAllTextAsync(_filePath, logLine + "\n");
        }
    }
}
