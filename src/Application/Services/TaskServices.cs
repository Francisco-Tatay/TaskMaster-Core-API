using TaskManagerPro.Application.Common.Interfaces;
using TaskManagerPro.Application.DTOs.Tasks;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Tasks;
using TaskManagerPro.TaskMasterPro.Domain;
using Task = TaskManagerPro.TaskMasterPro.Domain.Task;

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class TaskServices
{
    private readonly ITaskRepository _taskRepository;
    private readonly IIdGenerator _idGenerator;

    public TaskServices(ITaskRepository taskRepository, IIdGenerator idGenerator)
    {
        _taskRepository = taskRepository;
        _idGenerator = idGenerator;
    }

    public async Task<IEnumerable<TaskItemDto>> GetUserTasksAsync(Guid userId)
    {
        // 1. Obtenemos las ENTIDADES del repositorio
        var entities = await _taskRepository.GetAllByUserIdAsync(userId);

        // 2. MAPEO MANUAL: Convertimos de Entity a DTO
        // Esto quita el error de "Cannot convert IEnumerable<Task>..."
        return entities.Select(t => new TaskItemDto(
            t.Id,
            t.Title,
            t.Description
        ));
    }

    public async System.Threading.Tasks.Task CreateTaskAsync(TaskItemDto taskItemDto, Guid userId)
    {
        // Aquí ya lo tenías bien: conviertes DTO + UserId -> Entity
        Task entity = new Task()
        {
            Id = _idGenerator.NewId(), // Siempre genera un ID nuevo para creaciones
            Title = taskItemDto.Title,
            Description = taskItemDto.Description,
            UserId = userId,
            IsCompleted = false
        };
        await _taskRepository.AddAsync(entity);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskItemDto taskDto)
    {
        // FIX: No podemos mandar el DTO directo. Creamos la entidad.
        // En un nivel más pro, primero buscarías la tarea en la DB y luego la actualizarías,
        // pero para que compile y funcione el mapeo:
        var entity = await _taskRepository.GetByIdAsync(taskDto.Id);
        if (entity == null) return;
        entity.Title = taskDto.Title;
        entity.Description = taskDto.Description;
        await _taskRepository.UpdateAsync(entity);
    }

    // FIX: Cambiamos 'int' por 'Guid'
    public async System.Threading.Tasks.Task DeleteTaskAsync(Guid taskId)
    {
        await _taskRepository.DeleteAsync(taskId);
    }
}