using Entity.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Utilities.Implementations;
using Utilities.Interfaces;
using Web;
using Web.Config;
using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

//swager
builder.Services.AddCustomSwagger();


// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtAuthenticationService, JwtAuthenticatonService>();

builder.Services.AddHttpContextAccessor();
// CORS
builder.Services.AddCustomCors(builder.Configuration);

// extensión para la base de datos
builder.Services.AddDatabase(builder.Configuration);

// Repositorios y servicios
builder.Services.AddAppServices();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();


var app = builder.Build();
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

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

