using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class MySqlDatabaseFactory : IDatabaseFactory
    {
        /// <summary>
        /// Nombre del proveedor MySQL.
        /// </summary>
        public string ProviderName => "MySql";

        /// <summary>
        /// Configura las opciones para usar MySQL como proveedor.
        /// </summary>
        /// <param name="options">Builder de opciones de contexto.</param>
        /// <param name="connectionString">String de conexión a MySQL.</param>
        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseMySQL(connectionString);
        }
    }
}
