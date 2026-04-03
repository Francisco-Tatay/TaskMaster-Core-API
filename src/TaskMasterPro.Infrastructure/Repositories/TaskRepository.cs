
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskEntity?> GetByIdAsync(int id) => await _context.Task.FindAsync(id);

    public async Task<IEnumerable<TaskEntity>> GetAllByUserIdAsync(int userId)
    {
        return await _context.Task.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(TaskEntity task)
    {
        await _context.Task.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskEntity task)
    {
        _context.Task.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}