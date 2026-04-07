using Microsoft.AspNetCore.Mvc;
    using TaskManagerPro.TaskMasterPro.Application.DTOs.Tasks;
    using TaskManagerPro.TaskMasterPro.Application.Services;
    
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
    
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var tasks = await _taskServices.GetUserTasksAsync(userId);
    
            if (!tasks.Any())
                return NotFound($"User with ID {userId} has no tasks.");
    
            return Ok(tasks);
        }
    
        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> Create(Guid userId, [FromBody] TaskItemDto? taskDto)
        {
            if (taskDto == null)
                return BadRequest("Task data is required.");
    
            if (string.IsNullOrWhiteSpace(taskDto.Title))
                return BadRequest("Title is required.");
    
            await _taskServices.CreateTaskAsync(taskDto, userId);
    
            return Ok(new { message = "Task created successfully." });
        }
    
        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> Update(Guid taskId, [FromBody] TaskItemDto? taskDto)
        {
            if (taskDto == null)
                return BadRequest("Data is required.");
    
            if (taskId != taskDto.Id)
                return BadRequest("URL ID and Task Object ID mismatch.");
    
            await _taskServices.UpdateTaskAsync(taskDto);
    
            return Ok(new { message = "Task updated successfully." });
        }
    
        [HttpDelete("{taskId:guid}")]
        public async Task<IActionResult> Delete(Guid taskId)
        {
            await _taskServices.DeleteTaskAsync(taskId);
            return NoContent();
        }
    }