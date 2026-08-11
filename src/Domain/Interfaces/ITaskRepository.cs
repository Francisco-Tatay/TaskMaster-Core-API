using TaskManagerPro.TaskMasterPro.Domain;
using Task = TaskManagerPro.TaskMasterPro.Domain.Task;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface ITaskRepository
{
    // 1. Usamos GUID y devolvemos la ENTIDAD
    Task<Task?> GetByIdAsync(Guid id);

    // 2. Buscamos por GUID de usuario y devolvemos ENTIDADES
    Task<IEnumerable<Task>> GetAllByUserIdAsync(Guid userId);

    // 3. Guardamos la ENTIDAD
    System.Threading.Tasks.Task AddAsync(Task task);

    // 4. Actualizamos la ENTIDAD
    System.Threading.Tasks.Task UpdateAsync(Task task);

    // 5. Borramos usando el GUID
    System.Threading.Tasks.Task DeleteAsync(Guid id);

    // 6. Paginación: Recibe GUID y devuelve lista de ENTIDADES
    Task<List<Task>> GetTaskPagedAsync(Guid userId, int page, int pageSize);
}