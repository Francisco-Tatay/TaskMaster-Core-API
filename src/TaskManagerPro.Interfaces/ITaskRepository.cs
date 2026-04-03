using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TaskEntity>> GetAllByUserIdAsync(int userId);
    Task AddAsync(TaskEntity task);
    Task UpdateAsync(TaskEntity task);
    Task DeleteAsync(int id);
}