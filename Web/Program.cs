using Entity.Context;
using Entity.Contexts;
using Entity.Dtos.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json.Serialization;
using Utilities.Implementations;
using Utilities.Interfaces;
using Web;
using Web.Extensions;
using Web.Middleware;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//postgres migrations
//Add-Migration InitDbPostgres -StartupProject Web -Project Entity -Context ApplicationDbContextPostgreSql -OutputDir Migrations/PostgreSql
// sqlserver migrations
//Add-Migration InitDbSqlServer -StartupProject Web -Project Entity -Context ApplicationDbContextSqlServer -OutputDir Migrations/SqlServer
//mysql migrations
//Add-Migration InitDbMysql -StartupProject Web -Project Entity -Context ApplicationDbContextMySql -OutputDir Migrations/MySql


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Controllers
builder.Services.AddControllers();

//swager
builder.Services.AddCustomSwagger();

//Jwt y Cookie
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("Cookie"));

// extensión para la base de datos
builder.Services.AddDatabase(builder.Configuration);

// Repositorios y servicios
builder.Services.AddAppServices();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<DbContextMiddleware>();
MigrationManager.MigrateAllDatabases(app.Services, builder.Configuration);


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API v1");
    options.DocumentTitle = "Security API Docs";
    options.DefaultModelsExpandDepth(-1); // Ocultar esquema de modelos por defecto
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

// 3️⃣ HTTPS
app.UseHttpsRedirection();

// 4️⃣ CORS → siempre **antes de Authentication**
// para permitir cookies y headers cross-site
app.UseCors();

// 5️⃣ Autenticación (JWT + cookies)
app.UseAuthentication();

// 6️⃣ Autorización (policies, roles, etc.)
app.UseAuthorization();

// 7️⃣ Ruteo
app.MapControllers();

// 8️⃣ Ejecutar
app.Run();

