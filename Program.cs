// ... (Tus usings) ...

using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskManagerPro.TaskMasterPro.Infrastructure;
using TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services; // Agregado

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. INDISPENSABLE: Registrar los Controladores
        builder.Services.AddControllers();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MariaDbServerVersion(new Version(12, 2, 2));
        // Dile al contenedor de dependencias que sepa crear tu servicio
        builder.Services.AddScoped<GenerateToken>();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        builder.Services.AddAuthorization();

        // 2. REGISTRAR LOS REPOS (Ya lo tenías)
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // 3. INDISPENSABLE: Registrar el Servicio (El que inyectaste en el Controller)
        builder.Services.AddScoped<TaskServices>();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        // 4. INDISPENSABLE: Mapear los Controladores a las rutas
        app.MapControllers();

        app.Run();
    }
}