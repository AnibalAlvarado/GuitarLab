using Entity.Contexts;
using Entity.Database;
using Entity.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class MigrationManager
{
    public static void MigrateAllDatabases(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("MigrationManager");
        var factoryProvider = sp.GetRequiredService<DatabaseFactoryProvider>();

        // Leer targets desde configuración o usar el proveedor por defecto
        var targets = config.GetSection("MigrateOnStartupTargets").Get<string[]>() ?? Array.Empty<string>();

        if (targets.Length == 0)
        {
            var defaultProvider = config["DatabaseProvider"] ?? "SqlServer";
            targets = new[] { defaultProvider };
        }

        foreach (var raw in targets)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            try
            {
                if (!Enum.TryParse(raw, true, out DatabaseType provider))
                {
                    log.LogWarning("⚠ Proveedor desconocido: {Target}", raw);
                    continue;
                }

                var connectionString = config.GetConnectionString(provider.ToString());
                if (string.IsNullOrEmpty(connectionString))
                {
                    log.LogWarning("⚠ No hay cadena de conexión para {Target}, se omite.", provider);
                    continue;
                }

                log.LogInformation("➡ Migrando base de datos: {Provider}", provider);

                var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
                var factory = factoryProvider.GetFactory(provider.ToString());
                factory.Configure(dbOptions, connectionString);

                // ✅ Crear el contexto correcto según el proveedor
                using DbContext ctx = provider switch
                {
                    DatabaseType.SqlServer => new ApplicationDbContextSqlServer(dbOptions.Options),
                    DatabaseType.PostgreSql => new ApplicationDbContextPostgreSql(dbOptions.Options),
                    DatabaseType.MySql => new ApplicationDbContextMySql(dbOptions.Options),
                    _ => throw new InvalidOperationException($"Proveedor no soportado: {provider}")
                };

                ctx.Database.Migrate();

                log.LogInformation("✅ Migración completada para {Provider}", provider);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "❌ Error aplicando migraciones para {Target}", raw);
            }
        }
    }
}
