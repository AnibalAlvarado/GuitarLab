using Microsoft.OpenApi.Models;

namespace Web.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                // Documento principal de Swagger
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Security API",
                    Version = "v1",
                    Description = "API para gestión de usuarios, roles y permisos",
                    Contact = new OpenApiContact
                    {
                        Name = "Tu Nombre",
                        Email = "tuemail@ejemplo.com",
                        Url = new Uri("https://tusitio.com")
                    }
                });

                // Incluir documentación XML si está habilitada
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                // Soporte para enums como string
                options.UseInlineDefinitionsForEnums();

                // JWT Bearer auth
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Introduce el token JWT como: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                // Soporte para atributos como [FromHeader], [FromQuery], etc.
                options.SupportNonNullableReferenceTypes();
            });

            return services;
        }
    }
}
