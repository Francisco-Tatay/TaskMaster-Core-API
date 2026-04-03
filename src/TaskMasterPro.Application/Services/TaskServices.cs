using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskManagerPro.Interfaces; // Asegúrate de que el using sea el correcto

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class TaskServices
{
    // Usamos el nombre estándar: _taskRepository (sin la "i" al principio suele ser más común en C#)
    private readonly ITaskRepository _taskRepository;

    // AQUÍ ESTÁ EL CAMBIO: Inyectamos la Interfaz
    public TaskServices(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskEntity>> GetUserTasksAsync(int userId)
    {
        return await _taskRepository.GetAllByUserIdAsync(userId);
    }

    public async Task CreateTaskAsync(TaskEntity task)
    {
        // Regla de negocio: Por ejemplo, forzar que la tarea no empiece como completada
        task.IsCompleted = false; 
        await _taskRepository.AddAsync(task);
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        await _taskRepository.DeleteAsync(taskId);
    }
}