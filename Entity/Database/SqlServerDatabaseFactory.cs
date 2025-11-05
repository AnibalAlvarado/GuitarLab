using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class SqlServerDatabaseFactory : IDatabaseFactory
    {
        /// <summary>
        /// Nombre del proveedor SQL Server.
        /// </summary>
        public string ProviderName => "SqlServer";

        /// <summary>
        /// Configura las opciones para usar SQL Server como proveedor.
        /// </summary>
        /// <param name="options">Builder de opciones de contexto.</param>
        /// <param name="connectionString">String de conexión a SQL Server.</param>
        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseSqlServer(connectionString);
        }
    }
}
