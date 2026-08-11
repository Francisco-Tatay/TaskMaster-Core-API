namespace TaskManagerPro.TaskMasterPro.Application.DTOs.Tasks;

public interface CreateTaskDto
{
    string Title { get; init; }
    string? Description { get; init; }
}