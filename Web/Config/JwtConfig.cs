using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Web.Config
{
    public static class JwtConfig
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener configuración JWT
            var jwtSettings = configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            // Añadir autenticación JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero, // El token expira exactamente cuando se indica
                    RoleClaimType = ClaimTypes.Role
                };
            });

            // Añadir autorización con políticas basadas en roles
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EsAdmin", policy => policy.RequireRole("Administrador"));
                options.AddPolicy("EsUsuario", policy => policy.RequireRole("Usuario"));
            });

            return services;
        }
    }
}
