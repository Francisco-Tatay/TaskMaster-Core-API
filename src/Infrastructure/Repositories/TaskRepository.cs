using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Infrastructure;
using Task = TaskManagerPro.TaskMasterPro.Domain.Task;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    // 1. Buscamos por GUID
    public async Task<Task?> GetByIdAsync(Guid id) 
    {
        return await _context.Task.FindAsync(id);
    }

    // 2. Traemos todas las tareas de un usuario (Usando GUID)
    public async Task<IEnumerable<Task>> GetAllByUserIdAsync(Guid userId)
    {
        // El '==' aquí ya no falla porque ambos son Guid
        return await _context.Task
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    // 3. Guardar una ENTIDAD (No un DTO)
    public async System.Threading.Tasks.Task AddAsync(Task task)
    {
        await _context.Task.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    // 4. Actualizar una ENTIDAD
    public async System.Threading.Tasks.Task UpdateAsync(Task task)
    {
        _context.Task.Update(task);
        await _context.SaveChangesAsync();
    }

    // 5. Borrar por GUID
    public async System.Threading.Tasks.Task DeleteAsync(Guid id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    // 6. Paginación (Si la vas a usar, que devuelva ENTIDADES)
    public async Task<List<Task>> GetTaskPagedAsync(Guid userId, int page, int pageSize)
    {
        return await _context.Task
            .Where(t => t.UserId == userId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}