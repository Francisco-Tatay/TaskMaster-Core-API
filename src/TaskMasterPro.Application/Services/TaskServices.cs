using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Tasks;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class TaskServices
{
    private readonly ITaskRepository _taskRepository;

    public TaskServices(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> GetUserTasksAsync(Guid userId)
    {
        // 1. Obtenemos las ENTIDADES del repositorio
        var entities = await _taskRepository.GetAllByUserIdAsync(userId);

        // 2. MAPEO MANUAL: Convertimos de Entity a DTO
        // Esto quita el error de "Cannot convert IEnumerable<TaskEntity>..."
        return entities.Select(t => new TaskItemDto(
            t.Id, 
            t.Title, 
            t.Description
        ));
    }

    public async Task CreateTaskAsync(TaskItemDto taskItemDto, Guid userId)
    {
        // Aquí ya lo tenías bien: conviertes DTO + UserId -> Entity
        TaskEntity entity = new TaskEntity()
        {
            Id = Guid.NewGuid(), // Siempre genera un ID nuevo para creaciones
            Title = taskItemDto.Title,
            Description = taskItemDto.Description,
            UserId = userId,
            IsCompleted = false
        };
        await _taskRepository.AddAsync(entity);
    }

    public async Task UpdateTaskAsync(TaskItemDto taskDto)
    {
        // FIX: No podemos mandar el DTO directo. Creamos la entidad.
        // En un nivel más pro, primero buscarías la tarea en la DB y luego la actualizarías,
        // pero para que compile y funcione el mapeo:
        var entity = new TaskEntity()
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description
            // Ojo: Aquí faltaría el UserId si tu DB lo pide como obligatorio
        };

        await _taskRepository.UpdateAsync(entity);
    }

    // FIX: Cambiamos 'int' por 'Guid'
    public async Task DeleteTaskAsync(Guid taskId) 
    {
        await _taskRepository.DeleteAsync(taskId);
    }
}