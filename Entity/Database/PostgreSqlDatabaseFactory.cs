using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class PostgreSqlDatabaseFactory : IDatabaseFactory
    {
        /// <summary>
        /// Nombre del proveedor PostgreSQL.
        /// </summary>
        public string ProviderName => "PostgreSql";

        /// <summary>
        /// Configura las opciones para usar PostgreSQL como proveedor.
        /// </summary>
        /// <param name="options">Builder de opciones de contexto.</param>
        /// <param name="connectionString">String de conexión a PostgreSQL.</param>
        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseNpgsql(connectionString);
        }
    }
}
