using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public interface IDatabaseFactory
    {
        ///<summary>
        /// Configura las opciones del contexto de base de datos para un proveedor específico.
        /// </summary>
        /// <param name="options">Builder de opciones de contexto de base de datos a configurar.</param>
        /// <param name="connectionString">String de conexión para el proveedor.</param>
        void Configure(DbContextOptionsBuilder options, string connectionString);

        /// <summary>
        /// Nombre del proveedor de base de datos.
        /// </summary>
        string ProviderName { get; }
    }
}
