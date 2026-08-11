namespace TaskManagerPro.Application.DTOs.Tasks;

public record TaskItemDto(
    Guid Id,
    string Title,
    string? Description
);