using Microsoft.AspNetCore.Mvc;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.API;

[ApiController]
[Route("taskManagerPro/[controller]")]
public class TaskController : ControllerBase
{
    private readonly TaskServices _taskServices;

    public TaskController(TaskServices taskServices)
    {
        _taskServices = taskServices;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var tasks = await _taskServices.GetUserTasksAsync(userId);

        // Si la lista está vacía, avisamos
        if (!tasks.Any()) return NotFound($"User with ID {userId} has no tasks.");

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskEntity? task)
    {
        if (task == null) return BadRequest("Task data is required.");
        if (string.IsNullOrEmpty(task.Title)) return BadRequest("Title is required.");

        await _taskServices.CreateTaskAsync(task);

        // CreatedAtAction devuelve un 201 y la URL para ver el recurso
        return CreatedAtAction(nameof(GetByUserId), new { userId = task.UserId }, task);
    }

    [HttpPut("{taskid}")] // Corregido el cierre de llave
    public async Task<IActionResult> Update(int taskid, [FromBody] TaskEntity? task)
    {
        // 1. Validamos que el ID de la URL sea válido
        if (taskid <= 0) return BadRequest("Invalid Task ID.");

        // 2. Validamos que el cuerpo no sea nulo
        if (task == null) return BadRequest("Task data is required.");

        // 3. LA LÓGICA CLAVE: ¿Coincide el ID de la URL con el del objeto?
        if (taskid != task.Id)
        {
            return BadRequest("URL ID and Task Object ID mismatch.");
        }

        await _taskServices.UpdateTaskAsync(task);

        // Podrías devolver Ok o NoContent (204)
        return Ok(new { message = "Task updated successfully." });
    }

    [HttpDelete("{taskid}")]
    public async Task<IActionResult> Delete(int taskid)
    {
        if (taskid <= 0) return BadRequest("No task with the provided ID exists.");
        await _taskServices.DeleteTaskAsync(taskid);
        return NoContent();
    }
}