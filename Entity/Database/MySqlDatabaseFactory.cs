using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Entity.Database
{
    public class MySqlDatabaseFactory : IDatabaseFactory
    {
        /// <summary>
        /// Nombre del proveedor MySQL.
        /// </summary>
        public string ProviderName => "MySql";

        /// <summary>
        /// Configura las opciones para usar MySQL (Pomelo) como proveedor.
        /// </summary>
        /// <param name="options">Builder de opciones de contexto.</param>
        /// <param name="connectionString">String de conexión a MySQL/MariaDB.</param>
        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions
                    .EnableRetryOnFailure()  // reintentos automáticos
            );
        }
    }
}
