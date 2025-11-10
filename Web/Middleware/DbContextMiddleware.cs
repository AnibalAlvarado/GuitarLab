using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Entity.Database;
using Entity.Enums; // 👈 Aquí debe estar tu enum DatabaseType

namespace Web.Middleware
{
    /// <summary>
    /// Middleware que determina el proveedor de base de datos actual
    /// según el encabezado HTTP "X-DB-Provider" y lo almacena en el HttpContext.
    /// </summary>
    public class DbContextMiddleware
    {
        private readonly RequestDelegate _next;

        public DbContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 🧩 Leer el header personalizado
            string header = context.Request.Headers["X-DB-Provider"].ToString();

            // 🔍 Intentar convertirlo a un DatabaseType
            if (!Enum.TryParse(header, ignoreCase: true, out DatabaseType provider))
            {
                provider = DatabaseType.SqlServer; // valor por defecto
            }

            // 💾 Guardar en HttpContext.Items para que el DbContextFactory lo use
            context.Items["DbProvider"] = provider;

            // Continuar con el pipeline
            await _next(context);
        }
    }
}
