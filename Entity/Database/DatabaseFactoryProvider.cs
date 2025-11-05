using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class DatabaseFactoryProvider
    {
        private readonly Dictionary<string, IDatabaseFactory> _factories;

        /// <summary>
        /// Constructor que recibe todas las implementaciones de IDatabaseFactory disponibles.
        /// </summary>
        /// <param name="factories">Lista de fábricas de bases de datos disponibles.</param>
        public DatabaseFactoryProvider(IEnumerable<IDatabaseFactory> factories)
        {
            _factories = factories.ToDictionary(f => f.ProviderName, f => f);
        }

        /// <summary>
        /// Obtiene la fábrica correspondiente al proveedor especificado.
        /// </summary>
        /// <param name="providerName">Nombre del proveedor (debe coincidir con la configuración).</param>
        /// <returns>La fábrica correspondiente al proveedor.</returns>
        /// <exception cref="InvalidOperationException">Si el proveedor no está registrado.</exception>
        public IDatabaseFactory GetFactory(string providerName)
        {
            if (!_factories.TryGetValue(providerName, out var factory))
            {
                throw new InvalidOperationException($"Proveedor de base de datos no soportado: {providerName}");
            }

            return factory;
        }
    }
}
