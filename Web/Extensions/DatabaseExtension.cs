using Entity.Contexts;
using Entity.Database;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Utilities.Audit.Factory;
using Utilities.Audit.Services;

namespace Web.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // 🧱 Fábricas de bases de datos
            services.AddSingleton<IDatabaseFactory, SqlServerDatabaseFactory>();
            services.AddSingleton<IDatabaseFactory, PostgreSqlDatabaseFactory>();
            services.AddSingleton<IDatabaseFactory, MySqlDatabaseFactory>();

            // 📦 Proveedor de fábricas
            services.AddSingleton<DatabaseFactoryProvider>();

            // 🔧 Configuración del proveedor
            string databaseProvider = configuration["DatabaseProvider"];
            if (string.IsNullOrEmpty(databaseProvider))
            {
                throw new InvalidOperationException("El proveedor de base de datos no está especificado en la configuración.");
            }

            // 🔐 Cadena de conexión
            string connectionString = configuration.GetConnectionString(databaseProvider);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"La cadena de conexión para el proveedor '{databaseProvider}' no está configurada.");
            }

            // 🎯 Configuración del ApplicationDbContext
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var factoryProvider = serviceProvider.GetRequiredService<DatabaseFactoryProvider>();
                var factory = factoryProvider.GetFactory(databaseProvider);
                factory.Configure(options, connectionString);
            });

            // ✅ Registro para auditoría
            services.AddSingleton<Entity.Database.AuditDbContextFactory>();
            services.AddSingleton<IAuditStrategyFactory, AuditStrategyFactory>();
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }
    }
}
