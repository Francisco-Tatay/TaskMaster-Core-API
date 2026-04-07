namespace TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;

public record AuthRecordDto(
    Guid Id,
    string Email,
    string Token
    );