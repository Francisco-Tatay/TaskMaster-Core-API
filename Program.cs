using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Common.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;
using TaskManagerPro.TaskMasterPro.Infrastructure;
using TaskManagerPro.TaskMasterPro.Infrastructure.Auth;
using TaskManagerPro.TaskMasterPro.Infrastructure.Common;
using TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CONFIGURACIÓN DE BASE DE DATOS (MariaDB)
// =========================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MariaDbServerVersion(new Version(12, 2, 2));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// =========================================================================
// 2. REGISTRO DE REPOSITORIOS (Persistencia de Datos)
// =========================================================================
// Estos son los que hablan con la base de datos
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>(); // <-- Faltaba este cable

// =========================================================================
// 3. SERVICIOS DE INFRAESTRUCTURA (Herramientas Externas)
// =========================================================================
builder.Services.AddScoped<IPasswordHasher, BCryptHasher>();
builder.Services.AddScoped<ITokenService, GenerateTokenService>(); // <-- Registrado por interfaz
builder.Services.AddSingleton<IIdGenerator, IdGenerator>();

// =========================================================================
// 4. SERVICIOS DE APLICACIÓN (Lógica de Negocio / Casos de Uso)
// =========================================================================
// Estos orquestan la lógica que consumen los controladores
builder.Services.AddScoped<TaskServices>();
builder.Services.AddScoped<AuthService>(); // <-- Faltaba este cable para el AuthController
builder.Services.AddScoped<GenerateTokenService>();
// =========================================================================
// 5. CONFIGURACIÓN DE AUTENTICACIÓN Y JWT
// =========================================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Verifica expiración
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                       throw new InvalidOperationException("JWT Key missing!")))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Swagger/OpenAPI

// =========================================================================
// 6. PIPELINE DE LA APLICACIÓN (Middleware)
// =========================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// IMPORTANTE: El orden aquí importa. Primero Autenticación, luego Autorización.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();