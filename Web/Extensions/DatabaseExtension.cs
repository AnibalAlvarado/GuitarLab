using Entity.Contexts;
using Entity.Database;
using Entity.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utilities.Audit.Factory;
using Utilities.Audit.Services;
using Web.Infrastructure;

namespace Web.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // 🧱 Registrar fábricas de base de datos
            services.AddSingleton<IDatabaseFactory, SqlServerDatabaseFactory>();
            services.AddSingleton<IDatabaseFactory, PostgreSqlDatabaseFactory>();
            services.AddSingleton<IDatabaseFactory, MySqlDatabaseFactory>();

            services.AddSingleton<DatabaseFactoryProvider>();
            services.AddSingleton<DbContextFactory>();

            // ✅ Aquí está el truco:
            // Registramos ApplicationDbContext pero usando un factory dinámico que lee el provider actual
            services.AddScoped<ApplicationDbContext>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var config = sp.GetRequiredService<IConfiguration>();
                var factoryProvider = sp.GetRequiredService<DatabaseFactoryProvider>();

                // 🔍 Leer provider del contexto HTTP (si viene en el header)
                var providerFromRequest = httpContextAccessor.HttpContext?.Items["DbProvider"] as DatabaseType?;

                DatabaseType provider = providerFromRequest ??
                    (Enum.TryParse(config["DatabaseProvider"], ignoreCase: true, out DatabaseType parsed)
                        ? parsed
                        : DatabaseType.SqlServer);

                var connectionString = config.GetConnectionString(provider.ToString());
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException($"No se encontró cadena de conexión para '{provider}'.");

                var factory = factoryProvider.GetFactory(provider.ToString());
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                factory.Configure(optionsBuilder, connectionString);

                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbContextFactory");
                logger.LogInformation("🔄 Creando ApplicationDbContext dinámico con provider: {Provider}", provider);

                return new ApplicationDbContext(optionsBuilder.Options, config, httpContextAccessor);
            });

            // ✅ Auditoría
            services.AddSingleton<Entity.Database.AuditDbContextFactory>();
            services.AddSingleton<IAuditStrategyFactory, AuditStrategyFactory>();
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }
    }
}
