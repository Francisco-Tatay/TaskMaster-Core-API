using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface ITaskRepository
{
    // 1. Usamos GUID y devolvemos la ENTIDAD
    Task<TaskEntity?> GetByIdAsync(Guid id);

    // 2. Buscamos por GUID de usuario y devolvemos ENTIDADES
    Task<IEnumerable<TaskEntity>> GetAllByUserIdAsync(Guid userId);

    // 3. Guardamos la ENTIDAD
    Task AddAsync(TaskEntity task);

    // 4. Actualizamos la ENTIDAD
    Task UpdateAsync(TaskEntity task);

    // 5. Borramos usando el GUID
    Task DeleteAsync(Guid id);

    // 6. Paginación: Recibe GUID y devuelve lista de ENTIDADES
    Task<List<TaskEntity>> GetTaskPagedAsync(Guid userId, int page, int pageSize);
}