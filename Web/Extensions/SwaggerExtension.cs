using Entity.Database; // donde está tu enum DatabaseType
using Entity.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Web.Extensions
{
    public static class SwaggerService
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GuitarLab API",
                    Version = "v1",
                    Description = "API para gestión de usuarios y guitarristas",
                });

                // ============================================================
                // 🔒 JWT Bearer
                // ============================================================
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese el token JWT como: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // ============================================================
                // 🧩 CABECERA PERSONALIZADA: X-DB-Provider (con enum)
                // ============================================================
                c.AddSecurityDefinition("DbProvider", new OpenApiSecurityScheme
                {
                    Name = "X-DB-Provider",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = $"Selecciona el proveedor de base de datos. Valores posibles: {string.Join(", ", Enum.GetNames(typeof(DatabaseType)))}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "DbProvider"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
