using Entity.Contexts;
using Entity.Database;
using Entity.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Infrastructure
{
    public class DbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public ApplicationDbContext CreateDbContext()
        {
            // 🔍 Detectar proveedor dinámico desde HttpContext
            var providerFromRequest = _httpContextAccessor.HttpContext?.Items["DbProvider"] as string;

            // 📦 Usar el del header o el del appsettings
            DatabaseType provider = DatabaseType.SqlServer;

            if (!string.IsNullOrEmpty(providerFromRequest) &&
                Enum.TryParse(providerFromRequest, ignoreCase: true, out DatabaseType parsedFromHeader))
            {
                provider = parsedFromHeader;
            }
            else if (Enum.TryParse(_configuration["DatabaseProvider"], ignoreCase: true, out DatabaseType parsedFromConfig))
            {
                provider = parsedFromConfig;
            }

            // 🔐 Cadena de conexión
            string connectionString = _configuration.GetConnectionString(provider.ToString());
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"No se encontró cadena de conexión para el proveedor '{provider}'.");

            // 🧩 Resolver fábrica
            var factoryProvider = _serviceProvider.GetRequiredService<DatabaseFactoryProvider>();
            var factory = factoryProvider.GetFactory(provider.ToString());

            // ⚙️ Construir opciones
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            factory.Configure(optionsBuilder, connectionString);

            // 🚀 Crear instancia final
            return new ApplicationDbContext(optionsBuilder.Options, _configuration, _httpContextAccessor);
        }
    }
}
